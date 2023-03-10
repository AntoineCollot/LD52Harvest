using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class JumpController : MonoBehaviour
{
    [SerializeField, Range(1, 10)] float smallJumpGravMult = 2;
    [SerializeField, Range(1, 10)] float downwardMovementMult = 3;
    [SerializeField, Range(0, 0.5f)] float jumpBuffer = 0.3f;
    [SerializeField, Range(0, 0.5f)] float coyoteTime = 0.1f;
    [SerializeField, Range(0.5f, 5)] float jumpHeight = 2.2f;
    [SerializeField, Range(0.2f, 2)] float timeToJumpApex = 0.5f;
    [SerializeField, Range(5, 20)] float terminalFallVelocity = 20;
    [SerializeField, Range(0, 0.3f)] float snapToFullJumpTime = 0.15f;

    bool desireJump;
    bool isPressingJump;
    bool isJumping;
    Vector3 velocity;
    float gravityMult;
    float coyoteTimeCounter01;
    float jumpBufferCounter01;
    float jumpSpeed;
    float gravityScale;
    public UnityEvent onJump = new UnityEvent();

    //Snap to full jump
    float lastPressJumpTime;
    bool snapFullJump;

    [Header("Components")]
    Rigidbody body;
    GroundCaster ground;
    CustomGravity gravity;
    CompositeStateToken freezeGroundDetectionToken = new CompositeStateToken();

    [Header("Inputs")]
    protected InputMap inputs;

    public bool IsGoingUp => body.velocity.y > 0.01f;
    public bool IsFalling => body.velocity.y < -0.01f;
    public float InitialJumpSpeed => Mathf.Sqrt(-2f * Physics2D.gravity.y * gravityScale * jumpHeight);
    public float TerminalFallVelocity => terminalFallVelocity;
    public float VerticalVelocity => velocity.y;
    public bool IsGrounded => ground.isGrounded;

    void Awake()
    {
        inputs = new InputMap();
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        ground = GetComponentInChildren<GroundCaster>();
        gravity = GetComponent<CustomGravity>();
        ground.onGroundedStateChanged.AddListener(OnGroundedStateChanged);
        PlayerState.Instance.freezeGroundDetectionState.Add(freezeGroundDetectionToken);
    }

    void OnEnable()
    {
        inputs.Enable();
        inputs.Gameplay.Enable();

        inputs.Gameplay.Jump.performed += OnJump;
        inputs.Gameplay.Jump.canceled += OnJump;
    }

    void OnDisable()
    {
        inputs.Disable();

        inputs.Gameplay.Jump.performed -= OnJump;
        inputs.Gameplay.Jump.canceled -= OnJump;
    }

    private void Update()
    {
        //Check if we should freeze the inputs
        if (PlayerState.Instance.freezeInputsState.IsOn)
            desireJump = false;

        //Coyote time
        if (isJumping)
        {
            //Max out coyote time when jumping as we can't use it anymore
            coyoteTimeCounter01 = 1;
        }
        else if (!ground.isGrounded)
        {
            coyoteTimeCounter01 += Time.deltaTime / coyoteTime;
        }
        else
        {
            coyoteTimeCounter01 = 0;
        }

        if (desireJump)
        {
            jumpBufferCounter01 += Time.deltaTime / jumpBuffer;
            if (jumpBufferCounter01 >= 1)
            {
                //Cancel the buffer
                desireJump = false;
                jumpBufferCounter01 = 0;
            }
        }

        //Full jump detection
        if (isPressingJump && Time.time > lastPressJumpTime + snapToFullJumpTime)
            snapFullJump = true;
    }

    void FixedUpdate()
    {
        velocity = body.velocity;

        //Compute the gravityscale to get the correct jump duration
        gravityScale = (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex * Physics2D.gravity.y);

        if (desireJump)
        {
            TryJump();
            body.velocity = velocity;
            return;
        }

        if (IsGoingUp)
        {
            if ((isPressingJump && isJumping) || snapFullJump)
            {
                gravityMult = 1;
            }
            else
            {
                gravityMult = smallJumpGravMult;
            }
        }
        else if (IsFalling)
        {
            //Clamp falling velocity
            velocity.y = Mathf.Clamp(velocity.y, -terminalFallVelocity, Mathf.Infinity);

            gravityMult = downwardMovementMult;

            isJumping = false;
        }
        else
        {
            gravityMult = 1;
        }

        if (PlayerState.Instance.forceDefaultGravityState.IsOn)
            gravity.gravityScale = gravityScale;
        else
            gravity.gravityScale = gravityScale * gravityMult;

        if (PlayerState.Instance.freezePositionState.IsOn)
            velocity.y = 0;

        body.velocity = velocity;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            desireJump = true;
            isPressingJump = true;
            lastPressJumpTime = Time.time;
        }

        if (context.canceled)
        {
            isPressingJump = false;
            isJumping = false;
        }
    }

    void TryJump()
    {
        if (ground.isGrounded || (coyoteTimeCounter01 > 0 && coyoteTimeCounter01 < 1))
        {
            Jump(0);
        }
    }

    void Jump(float jumpHeightModifier)
    {
        jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * gravityScale * (jumpHeight + jumpHeightModifier));

        if (velocity.y > 0f)
        {
            //Make sure we don't add multiple jump speed
            jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0);
        }
        else if (velocity.y < 0)
        {
            //Cancel the falling velocity
            jumpSpeed += Mathf.Abs(body.velocity.y);
        }

        isJumping = true;
        desireJump = false;
        snapFullJump = false;

        velocity.y += jumpSpeed;

        //Audio
        SFXManager.PlaySound(GlobalSFX.Jump);

        freezeGroundDetectionToken.SetOn(true);
        Invoke("AllowGroundDetection",0.25f);

        onJump.Invoke();
    }

    void AllowGroundDetection()
    {
        freezeGroundDetectionToken.SetOn(false);
    }

    private void OnGroundedStateChanged(bool isGrounded)
    {
        //landing
        if (isGrounded)
        {
            isJumping = false;
            snapFullJump = false;
            SFXManager.PlaySound(GlobalSFX.Land);
        }
    }
}

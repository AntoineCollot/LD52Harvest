using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundCaster : MonoBehaviour
{
    [SerializeField, Range(0, 0.5f)] float castDistance = 0.1f;
    [SerializeField, Range(0, 1)] float castRadius = 0.3f;
    [SerializeField] LayerMask groundLayers;

    float lastUpdateTime;
    RaycastHit2D[] circleCastHits = new RaycastHit2D[10];
    bool lastGroundedState = false;
    Rigidbody body;

    //State
    public class GroundedEvent : UnityEvent<bool> { }
    public GroundedEvent onGroundedStateChanged = new GroundedEvent();

    public Vector3 groundPosition;
    public bool isGrounded { get; private set; }

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody>();
    }

    public void CastGround()
    {
        //Don't cast multiple times per fixed frame
        if (lastUpdateTime >= Time.fixedTime)
            return;

        lastUpdateTime = Time.fixedTime;

        if (PlayerState.Instance.freezeGroundDetectionState.IsOn)
        {
            isGrounded = false;
        }
        //Can't be grounded when going upward
        else if (!lastGroundedState && body.velocity.y > 0.01f)
        {
            isGrounded = false;
        }
        else
        {
            bool forward = Physics.Raycast(transform.position + transform.forward * castRadius, Vector3.down, out RaycastHit hitF,castDistance, groundLayers);
            bool left = Physics.Raycast(transform.position - transform.right * castRadius, Vector3.down, out RaycastHit hitL,castDistance, groundLayers);
            bool right = Physics.Raycast(transform.position + transform.right * castRadius, Vector3.down, out RaycastHit hitR,castDistance, groundLayers);
            bool back = Physics.Raycast(transform.position - transform.forward * castRadius, Vector3.down, out RaycastHit hitB,castDistance, groundLayers);
            if (forward || left || right || back)
            {
                isGrounded = true;

                groundPosition = Vector3.zero;
                int hitCount = 0;
                if (hitF.collider != null)
                {
                    groundPosition += hitF.point;
                    hitCount++;
                }
                if (hitL.collider != null)
                {
                    groundPosition += hitL.point;
                    hitCount++;
                }
                if (hitR.collider != null)
                {
                    groundPosition += hitR.point;
                    hitCount++;
                }
                if (hitB.collider != null)
                {
                    groundPosition += hitB.point;
                    hitCount++;
                }
                groundPosition /= hitCount;
            }
            else
                isGrounded = false;
        }

        //Event
        if (lastGroundedState != isGrounded)
        {
            onGroundedStateChanged.Invoke(isGrounded);
            if (isGrounded)
                SFXManager.PlaySound(GlobalSFX.Land);
        }

        lastGroundedState = isGrounded;
    }

    void SnapToGround()
    {
        if (!isGrounded)
            return;
        //Snap to ground
        Vector3 bodyPosition = body.transform.position;
        bodyPosition.y = groundPosition.y;
        body.transform.position = bodyPosition;
        Vector3 velocity= body.velocity;
        velocity.y = 0;
        body.velocity = velocity;
    }

    private void Update()
    {
        SnapToGround();
    }

    private void FixedUpdate()
    {
        CastGround();
        SnapToGround();
    }

    private void OnDrawGizmosSelected()
    {
        if (isGrounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.down * castDistance);
        Gizmos.DrawWireSphere(transform.position+ Vector3.down * castDistance, castRadius);
    }
}

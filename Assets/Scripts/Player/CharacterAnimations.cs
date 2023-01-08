using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    JumpController jump;
    MovementController move;
    public GroundCaster groundCaster;
    public Transform armature;
    Animator anim;

    Vector3 currentDirection = Vector3.forward;
    Vector3 targetDirection;
    public float rotationSpeed = 360;

    private void Awake()
    {
        jump = GetComponentInParent<JumpController>();
        move = GetComponentInParent<MovementController>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        groundCaster.onGroundedStateChanged.AddListener(OnGroundedStateChanged);
        jump.onJump.AddListener(OnJump);
        GetComponentInParent<KillController>().onKill.AddListener(OnKill);
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("MoveSpeed", move.NormalizedMoveSpeed);
        anim.SetFloat("VerticalSpeed", jump.VerticalVelocity);

        if (move.NormalizedMoveSpeed > 0.1f)
            targetDirection = move.MoveDirection;

        float rotationStep = Mathf.Deg2Rad * rotationSpeed * Time.deltaTime;
        currentDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotationStep, 0);
        currentDirection.y = 0;
        transform.LookAt(transform.position + currentDirection);
    }

    private void OnGroundedStateChanged(bool isGrounded)
    {
        anim.SetBool("IsGrounded", isGrounded);

        //landing
        if (isGrounded)
        {
            StartCoroutine(LandingScaleAnim(0.2f));
        }
    }

    IEnumerator LandingScaleAnim(float time)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;

            float scaleFactor = Mathf.Lerp(0, 0.3f, Mathf.PingPong(Mathf.Clamp01(t) * 2, 1));
            armature.transform.localScale = new Vector3(1, 1 + scaleFactor, 1 - scaleFactor) * 100;

            yield return null;
        }
    }

    void OnJump()
    {
        anim.SetTrigger("Jump");
        anim.SetFloat("VerticalSpeed", jump.VerticalVelocity);
    }

    void OnKill()
    {
        anim.ResetTrigger("Jump");
        anim.SetTrigger("Attack");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, currentDirection*10);
        Gizmos.color = Color.red;
        if (move != null && move.NormalizedMoveSpeed > 0.1f)
            Gizmos.DrawRay(transform.position, move.MoveDirection*10);
    }
#endif
}

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
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, castDistance, groundLayers))
            //if(Physics.BoxCast(transform.position, Vector3.one * castRadius, Vector3.down,out RaycastHit hit,Quaternion.identity, castDistance, groundLayers))
            {
                isGrounded = true;
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

    private void FixedUpdate()
    {
        CastGround();
    }

    private void OnDrawGizmosSelected()
    {
        if (isGrounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.blue;
        Vector3 castTargetPos = transform.position + Vector3.down * castDistance;
        Gizmos.DrawLine(transform.position, castTargetPos);
        Gizmos.DrawWireCube(castTargetPos, Vector3.one * castRadius);
    }
}

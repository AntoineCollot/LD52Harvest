using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VillagerPathWalker : MonoBehaviour
{
    public SplineContainer spline;
    SplinePath path;
    public float moveSpeed = 0.05f;
    float progress = 0f;
    float pathLength;
    Vector3 previousPos;
    Animator anim;
    Villager villager;

    public LayerMask groundLayers;

    void Start()
    {
        anim = GetComponent<Animator>();
        villager = GetComponent<Villager>();

        if (spline != null)
        {
            var matrix = spline.transform.localToWorldMatrix;
            path = new SplinePath(new[]
            {
             new SplineSlice<Spline>(spline.Splines[0], new SplineRange(0, spline.Splines[0].Count+1), matrix),
            });
            pathLength = path.GetLength();
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsClimbing", false);
        }
    }

    void Update()
    {
        if (!villager.isDead && spline!=null)
        {
            Walk();
        }
    }

    void Walk()
    {
        Vector3 pos = path.EvaluatePosition(progress);
        transform.position = pos;

        progress += moveSpeed / pathLength * Time.deltaTime;
        progress %= 1;

        //Ground
        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out RaycastHit hit, 100, groundLayers))
        {
            transform.position = hit.point;
        }

        //Look at
        Vector3 deltaPos = transform.position - previousPos;
        Vector3 lookAtDir = deltaPos;
        lookAtDir.y = 0;
        transform.LookAt(transform.position + lookAtDir);

        anim.SetBool("IsClimbing",Mathf.Abs(deltaPos.y)> lookAtDir.magnitude);

        previousPos = transform.position;
    }
}

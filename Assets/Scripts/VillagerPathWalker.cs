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

    public LayerMask groundLayers;

    void Start()
    {
        var matrix = spline.transform.localToWorldMatrix;
        path = new SplinePath(new[]
        {
             new SplineSlice<Spline>(spline.Splines[0], new SplineRange(0, spline.Splines[0].Count+1), matrix),
        });
        pathLength = path.GetLength();
    }

    void Update()
    {
        Vector3 pos = path.EvaluatePosition(progress);
        transform.position = pos;

        progress += moveSpeed / pathLength * Time.deltaTime;
        progress %= 1;

        //Ground
        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out RaycastHit hit,100, groundLayers))
        {
            transform.position = hit.point;
        }

        //Look at
        Vector3 lookAtDir = transform.position - previousPos;
        lookAtDir.y = 0;
        transform.LookAt(transform.position + lookAtDir);
        previousPos = pos;
    }
}

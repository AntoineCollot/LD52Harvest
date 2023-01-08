using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreventFalling : MonoBehaviour
{
    public float maxDistFromCenter = 20;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = 0;

        if(transform.position.magnitude>maxDistFromCenter)
        {
            pos.Normalize();
            pos *= maxDistFromCenter;
            pos.y = transform.position.y;
            transform.position = pos;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector3.zero, maxDistFromCenter);
    }
#endif
}

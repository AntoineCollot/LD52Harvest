using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnderneathGizmos : MonoBehaviour
{
    public LayerMask groundLayers;
    public Transform gizmos;
    public float minDistance;

    float maxAlpha;
    const float SMOOTH_ALPHA = 0.075f;
    float refAlpha;
    float alpha;

    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        Color c = sprite.color;
        maxAlpha = c.a;
        c.a = 0;
        sprite.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100, groundLayers))
        {
            if (Vector3.Distance(transform.position, hit.point) > minDistance)
                alpha = Mathf.SmoothDamp(alpha, 1, ref refAlpha, SMOOTH_ALPHA);
            else
                alpha = Mathf.SmoothDamp(alpha, 0, ref refAlpha, SMOOTH_ALPHA);

            Color c = sprite.color;
            c.a = alpha * maxAlpha;
            sprite.color = c;
            gizmos.position = hit.point;
        }
    }
}

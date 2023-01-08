using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillController : MonoBehaviour
{
    GroundCaster ground;
    public float areaRadius = 2;
    public LayerMask characterLayers;

    public UnityEvent onKill = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        ground = GetComponentInChildren<GroundCaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ground.isGrounded)
            return;

        Collider[] hitCol = Physics.OverlapSphere(transform.position, areaRadius, characterLayers);
        foreach(Collider hit in hitCol)
        {
            if(hit.TryGetComponent<Villager>(out Villager villager))
            {
                if(villager.Kill())
                {
                    onKill.Invoke();
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
#endif
}

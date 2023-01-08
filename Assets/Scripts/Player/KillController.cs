using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillController : MonoBehaviour
{
    GroundCaster ground;
    public float areaRadius = 2;
    public LayerMask characterLayers;
    CompositeStateToken freezePositionToken = new CompositeStateToken();
    const float FREEZE_TIME = 0.7f;
    bool isKilling = false;

    public UnityEvent onKill = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        ground = GetComponentInChildren<GroundCaster>();
        PlayerState.Instance.freezeInputsState.Add(freezePositionToken);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ground.isGrounded || isKilling)
            return;

        Collider[] hitCol = Physics.OverlapSphere(transform.position, areaRadius, characterLayers);
        foreach(Collider hit in hitCol)
        {
            if(hit.TryGetComponent<Villager>(out Villager villager))
            {
                if(villager.Kill())
                {
                    freezePositionToken.SetOn(true);
                    Invoke("UnfreezePlayer", FREEZE_TIME);
                    isKilling = true;
                    SFXManager.PlaySound(GlobalSFX.Attack);
                    onKill.Invoke();
                }
            }
        }
    }

    void UnfreezePlayer()
    {
        isKilling = false;
        freezePositionToken.SetOn(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
#endif
}

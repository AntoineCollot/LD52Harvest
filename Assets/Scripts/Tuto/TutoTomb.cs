using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoTomb : MonoBehaviour
{
    public GameObject content;

    private void OnTriggerEnter(Collider other)
    {
        content.SetActive(true);
        SFXManager.PlaySound(GlobalSFX.GhostApparition);
    }
}

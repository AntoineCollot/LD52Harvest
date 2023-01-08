using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    bool isDead;
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Kill()
    {
        if (isDead)
            return false;

        Invoke("KillAnimDelayed", 0.3f);
        isDead = true;
        return true;
    }

    void KillAnimDelayed()
    {
        anim.SetBool("IsDead", true);
    }
}

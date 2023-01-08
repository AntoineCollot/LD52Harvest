using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Villager : MonoBehaviour
{
   public bool isDead { get; private set; }
    Animator anim;

    [Header("LifeTime")]
    public TextMeshProUGUI text;
   [Range(0, 60)] public float lifetimeCycleOffset = 0;
   [Range(0, 60)] public float lifetimeCycleLength = 0;
   public float lifetime01 { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        lifetime01 = lifetimeCycleOffset / lifetimeCycleLength;
    }

    private void Update()
    {
        if (isDead)
            return;

        lifetime01 += Time.deltaTime / lifetimeCycleLength;
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

    public static string[] Names = new string[]
    {
        "Ted",
        "Mia",
        "Gus",
        "Jax",
        "Paco",
        "Lucky",
        "Arno",
        "Ace",
        "Ivy",
        "Jazz",
        "Leo",
        "Prince",
        "Tito",
        "Buddy"
    };
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Villager : MonoBehaviour
{
    public int id;

    public bool isDead { get; private set; }
    Animator anim;
    public TextMeshProUGUI text;
    MovementController playerMovement;

    [Header("Fade")]
    [Range(0, 30)] public float UIDistance = 10;
    CanvasGroup canvasGroup;
    float refAlpha;
    const float SMOOTH_ALPHA = 0.1f;

    [Header("LifeTime")]
    public Slider slider;
    public Image skullHighlight;
    [Range(0, 60)] public float lifetimeCycleOffset = 0;
    [Range(0, 60)] public float lifetimeCycleLength = 0;
    public float lifetime01 { get; private set; }
    bool isGoingUp = true;

    public const float PERFECT_KILL_THRESHOLD = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        lifetime01 = lifetimeCycleOffset / lifetimeCycleLength;
        UpdateName();
        playerMovement = FindObjectOfType<MovementController>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateName();
    }
#endif

    private void Update()
    {
        UpdateUIAlpha();

        if (isDead)
            return;

        if (isGoingUp)
            lifetime01 += Time.deltaTime / lifetimeCycleLength;
        else
            lifetime01 -= Time.deltaTime / lifetimeCycleLength;

        if (lifetime01 >= 1)
        {
            lifetime01 = 1;
            isGoingUp = false;
        }
        else if (lifetime01 <= 0)
        {
            lifetime01 = 0;
            isGoingUp = true;
        }

        slider.value = lifetime01;

        skullHighlight.enabled = lifetime01 > PERFECT_KILL_THRESHOLD;
    }

     void UpdateUIAlpha()
    {
        //Fade
        float targetAlpha = 0;
        if (!isDead && Vector3.Distance(playerMovement.transform.position, transform.position) < UIDistance)
            targetAlpha = 1;
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, targetAlpha, ref refAlpha, SMOOTH_ALPHA);
        canvasGroup.gameObject.SetActive(canvasGroup.alpha > 0);
    }

    void UpdateName()
    {
        text.text = Names[id % Names.Length];
    }

    public bool Kill()
    {
        if (isDead)
            return false;

        Invoke("KillAnimDelayed", 0.3f);
        isDead = true;

        KillManager.Instance.OnCharacterKill(lifetime01, transform.position);

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

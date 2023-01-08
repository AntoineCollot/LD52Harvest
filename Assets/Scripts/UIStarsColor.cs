using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStarsColor : MonoBehaviour
{
    public Graphic[] stars;
    public float[] minScore;
    const float TIMER_PER_STAR = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(DisplayStars());
    }

    IEnumerator DisplayStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (KillManager.Instance.score >= minScore[i])
                stars[i].color = Color.white;
            else
                stars[i].color = new Color(1, 1, 1, 0.035f);

            stars[i].transform.localScale = Vector3.zero;
        }

        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < stars.Length; i++)
        {
            if (KillManager.Instance.score >= minScore[i])
            {
                SFXManager.PlaySound(GlobalSFX.Star);
            }

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / TIMER_PER_STAR;

                stars[i].transform.localScale = Vector3.one * Curves.QuadEaseOut(0, 1, Mathf.Clamp01(t));

                yield return null;
            }

        }
    }
}

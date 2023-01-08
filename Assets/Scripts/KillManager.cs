using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillManager : MonoBehaviour
{
    public static KillManager Instance;
    public int killCount { get; private set; }
    public float score { get; private set; }
    public int soulsCount { get; private set; }
    public int diedFromOldAgeCount { get; private set; }

    public GameObject killFXPrefab;
    public TextMeshProUGUI soulsText;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        Instance = this;
        soulsCount = FindObjectsOfType<Villager>().Length;
        soulsText.text = $"0/{soulsCount.ToString()}";
    }

    public void OnDieFromOldAge()
    {
        diedFromOldAgeCount++;

        UpdateTexts();

        if (killCount + diedFromOldAgeCount >= soulsCount)
            GameManager.Instance.GameOver();
    }

    public void OnCharacterKill(float lifetime01, Vector3 position)
    {
        killCount++;
        float points = Mathf.Lerp(0, 100, lifetime01 / Villager.PERFECT_KILL_THRESHOLD);
        score += points;

        UpdateTexts();

        GameObject killFx = Instantiate(killFXPrefab, position, Quaternion.identity, transform);
        killFx.GetComponentInChildren<TextMeshProUGUI>().text = "+" + points.ToString("N0");

        if (killCount+diedFromOldAgeCount >= soulsCount)
            GameManager.Instance.GameOver();
    }

    void UpdateTexts()
    {
        scoreText.text = score.ToString("N0");
        if(diedFromOldAgeCount==0)
            soulsText.text = $"{killCount}/{soulsCount.ToString()}";
        else
            soulsText.text = $"{killCount}/{soulsCount.ToString()} (-{diedFromOldAgeCount})";

    }
}

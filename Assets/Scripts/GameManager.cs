using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent onGameOver = new UnityEvent();

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }


    public void GameOver()
    {
        onGameOver.Invoke();
    }
}

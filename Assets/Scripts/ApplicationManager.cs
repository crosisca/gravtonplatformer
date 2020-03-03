using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    static ApplicationManager instance;

    public static ApplicationManager Instance => instance ?? (instance = FindObjectOfType<ApplicationManager>());

    GameManager gameManager;

    private void Awake ()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start ()
    {
        GoFromMenuToGame(1, 1);
    }

    public void GoFromMenuToGame(int worldNumber, int levelNumber)
    {
        gameManager = Instantiate(Resources.Load<GameManager>(Constants.GameMangerPrefabPath));

        GameManager.Instance.StartNewLevel(worldNumber, levelNumber);
        GameManager.Instance.OnLevelGoalReached += OnLevelGoalReached;
        GameManager.Instance.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnLevelGoalReached (int worldNumber, int levelNumber)
    {
        //TODO show end game ui
    }

    void OnLevelCompleted (int worldNumber, int levelNumber)
    {
        GoFromGameToMenu();

        //HACK simulate clicking on next level
        GoFromMenuToGame(worldNumber, ++levelNumber);
    }

    public void GoFromGameToMenu()
    {
        GameManager.Instance.UnloadCurrentLevel();

        GameManager.Instance.OnLevelGoalReached -= OnLevelGoalReached;
        GameManager.Instance.OnLevelCompleted -= OnLevelCompleted;
        
        Destroy(gameManager.gameObject);
    }

    void Update ()
    {
        //Debug load level 1-9 pressing keys
        for (int i = 49; i < 58; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
                GoFromMenuToGame(1, i - 48);
        }
    }
}

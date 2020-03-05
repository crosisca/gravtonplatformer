using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class ApplicationManager : MonoBehaviour
{
    static ApplicationManager instance;

    public static ApplicationManager Instance => instance ?? (instance = FindObjectOfType<ApplicationManager>());

    GameManager gameManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        new GameObject().AddComponent<ApplicationManager>();
    }

    private void Awake ()
    {
        gameObject.name = "[ApplicationManager]";
        DontDestroyOnLoad(gameObject);
    }

    private void Start ()
    {
        Button[] lvlButtons = GameObject.Find("World1Levels").transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < lvlButtons.Length; i++)
        {
            int n = i+1;
            lvlButtons[i].onClick.AddListener(() => OnLevelButtonClicked(n));
        }

        GameObject.Find("Button_Unload").GetComponent<Button>().onClick.AddListener(ForceUnloadCurrentLevel);
    }

    public void OnLevelButtonClicked(int lvlNumber)
    {
        GoFromMenuToGame(1, lvlNumber);
    }

    public void GoFromMenuToGame(int worldNumber, int levelNumber)
    {
        if (gameManager != null)//level is already loaded
            return;
        gameManager = Instantiate(Resources.Load<GameManager>(Constants.GameManagerPrefabPath));

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

        Timing.CallDelayed(1, () => GoFromMenuToGame(worldNumber, ++levelNumber));
    }

    public void GoFromGameToMenu()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.OnLevelGoalReached -= OnLevelGoalReached;
        GameManager.Instance.OnLevelCompleted -= OnLevelCompleted;

        GameManager.Instance.UnloadCurrentLevel();
        
        Destroy(gameManager.gameObject);
        gameManager = null;
    }

    public void ForceUnloadCurrentLevel()
    {
        GoFromGameToMenu();
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

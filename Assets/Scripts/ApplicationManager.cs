using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    static ApplicationManager instance;

    public static ApplicationManager Instance => instance ?? (instance = FindObjectOfType<ApplicationManager>());

    public GameManager GameManager { get; private set; }

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
        GameObject.Find("Button_Pause").GetComponent<Button>().onClick.AddListener(ForceTogglePause);
    }

    public void OnLevelButtonClicked(int lvlNumber)
    {
        StartGameLevel(1, lvlNumber);
    }

    public void StartGameLevel(int worldNumber, int levelNumber)
    {
        if (GameManager != null)//level is already loaded
            return;

        GameManager = Instantiate(Resources.Load<GameManager>(Constants.GameManagerPrefabPath));

        GameManager.Instance.StartNewLevel(worldNumber, levelNumber);
        GameManager.Instance.OnLevelGoalReached += OnLevelGoalReached;
        GameManager.Instance.OnLevelFinished += OnLevelFinished;
    }

    private void OnLevelGoalReached (int worldNumber, int levelNumber)
    {
        //TODO show end game ui
    }

    void OnLevelFinished (int worldNumber, int levelNumber)
    {
        EndGameLevel();

        Timing.CallDelayed(1, () => StartGameLevel(worldNumber, ++levelNumber));
    }

    public void EndGameLevel()
    {
        if (GameManager == null)
            return;

        GameManager.OnLevelGoalReached -= OnLevelGoalReached;
        GameManager.OnLevelFinished -= OnLevelFinished;

        GameManager.Terminate();
        GameManager = null;
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Application"));
    }

    public void ForceUnloadCurrentLevel()
    {
        EndGameLevel();
    }

    public void ForceTogglePause ()
    {
        GameManager?.TogglePause();
    }

    void Update ()
    {
        //Debug load level 1-9 pressing keys
        for (int i = 49; i < 58; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
                StartGameLevel(1, i - 48);
        }
    }
}

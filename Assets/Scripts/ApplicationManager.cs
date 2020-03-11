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

    LevelSelectionPanel levelSelectionPanel;

    public event Action OnGameSessionStarted;
    public event Action OnGameSessionFinished;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        new GameObject().AddComponent<ApplicationManager>();
    }

    private void Awake ()
    {
        gameObject.name = "[ApplicationManager]";
        DontDestroyOnLoad(gameObject);
        levelSelectionPanel = FindObjectOfType<LevelSelectionPanel>();
    }

    private void Start ()
    {
        levelSelectionPanel.OnLevelSelected += OnLevelSelected;
    }

    public void OnLevelSelected(int worldNumber, int levelNumber)
    {
        StartGameLevel(worldNumber, levelNumber);
        
        levelSelectionPanel.Close();
    }

    void StartGameLevel(int worldNumber, int levelNumber)
    {
        if (GameManager != null)//level is already loaded
            return;

        GameManager = Instantiate(Resources.Load<GameManager>(Constants.GameManagerPrefabPath));

        GameManager.StartNewLevel(worldNumber, levelNumber);

        OnGameSessionStarted?.Invoke();
    }

    public void GoToMenu()
    {
        EndGameLevel();

        ShowMenu();
    }

    public void GoToNextLevel ()
    {
        int curentWorldNumber = GameManager.Instance.loadedWorldNumber;
        int nextLevelNumber = GameManager.Instance.loadedLevelNumber + 1;
        EndGameLevel();

        Timing.CallDelayed(Timing.WaitUntilTrue(() => GameManager == null), () => StartGameLevel(curentWorldNumber, nextLevelNumber));
    }

    public void EndGameLevel()
    {
        if (GameManager == null)
            return;

        OnGameSessionFinished?.Invoke();
        
        GameManager.Terminate();
    }

    void ShowMenu()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Application"));
        levelSelectionPanel.Open();
    }

    public void DestroyGameManager()
    {
        Destroy(GameManager.gameObject);
        GameManager = null;
    }
}

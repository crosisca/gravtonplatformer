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

    public static bool IsAnyLevelLoaded = false;

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
        Debug.Log($"StartGameLevel {worldNumber}-{levelNumber}");
        if (IsAnyLevelLoaded) //level is already loaded
        {
            Debug.LogError("Level already loaded. Ignoring");
            return;
        }

        GameManager = Instantiate(Resources.Load<GameManager>(Constants.GameManagerPrefabPath));

        GameManager.StartNewLevel(worldNumber, levelNumber);

        IsAnyLevelLoaded = true;

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

        Debug.Log($"Go to next level: {curentWorldNumber}-{nextLevelNumber}");

        Timing.RunCoroutine(WaitPreviousLevelUnloadAndLoadNext(curentWorldNumber, nextLevelNumber));
    }

    IEnumerator<float> WaitPreviousLevelUnloadAndLoadNext(int worldNumber, int lvlNumber)
    {
        while (IsAnyLevelLoaded)
        {
            Debug.Log("Waiting previous level to unload");
            yield return 0;
        }
        StartGameLevel(worldNumber, lvlNumber);
    }

    public void EndGameLevel()
    {
        if (!IsAnyLevelLoaded)
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
        Debug.Log("Destroying GameManager");
        Destroy(GameManager.gameObject);

        IsAnyLevelLoaded = false;
    }
}

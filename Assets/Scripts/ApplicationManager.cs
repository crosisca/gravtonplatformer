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

    public void StartGameLevel(int worldNumber, int levelNumber)
    {
        if (GameManager != null)//level is already loaded
            return;

        GameManager = Instantiate(Resources.Load<GameManager>(Constants.GameManagerPrefabPath));

        GameManager.StartNewLevel(worldNumber, levelNumber);
        GameManager.OnLevelFinished += OnLevelFinished;

        OnGameSessionStarted?.Invoke();
    }
    
    void OnLevelFinished (int worldNumber, int levelNumber)
    {
        EndGameLevel();

        ShowMenu();//TODO vai ficar no botao de home ;)
    }

    public void EndGameLevel()
    {
        if (GameManager == null)
            return;

        OnGameSessionFinished?.Invoke();

        GameManager.OnLevelFinished -= OnLevelFinished;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    Unset,
    Paused,
    Loading,
    WaitingToStart,
    Running,
    GoalReached,
    Failed,
    Unloading,
    Reseting,
}

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance => ApplicationManager.Instance.GameManager;

    public Scene activeLevel;

    public int loadedWorldNumber;
    public int loadedLevelNumber;

    public PlayerController Player { get; private set; }
    PlayerCamera playerCamera;

    CoroutineHandle startLevelCoroutine;

    List<Action> registeredUpdates = new List<Action>();
    List<Action> registeredFixedUpdates = new List<Action>();
    List<Action> registeredLateUpdates = new List<Action>();

    public event Action OnLevelLoadCompleted;
    public event Action OnLevelStarted;
    /// <summary>
    /// WorldNumber, LevelNumber
    /// </summary>
    public event Action<int,int> OnLevelGoalReached;
    /// <summary>
    /// WorldNumber, LevelNumber
    /// </summary>
    public event Action<int, int> OnLevelFailed;

    const string coroutinesTag = "gameManager";

    public bool IsPaused { get; private set;}
    public event Action<bool> OnPauseChanged;

    CoroutineHandle unloadCoroutine;

    IngameHUDPanel hud;
    LevelFailedPanel levelFailedPanel;
    LevelCompletedPanel levelCompletedPanel;

    Canvas canvas;

    private void OnPlayerDeath ()
    {
        Timing.CallDelayed(1, LevelFailed);
    }

    void Start()
    {
        StartRotator();

        Timing.RunCoroutine(CustomUpdate(), Segment.Update, coroutinesTag);
        Timing.RunCoroutine(CustomFixedUpdate(), Segment.FixedUpdate, coroutinesTag);
        Timing.RunCoroutine(CustomLateUpdate(), Segment.LateUpdate, coroutinesTag);
    }

    #region CustomUpdater
    public void AddUpdate(Action func)
    {
        registeredUpdates.Add(func);
    }

    public void RemoveUpdate (Action func)
    {
        registeredUpdates.Remove(func);
    }

    public IEnumerator<float> CustomUpdate ()
    {
        yield return Timing.WaitForOneFrame;
        while (gameObject != null)
        {
            if (gameObject.activeInHierarchy && enabled)
                foreach (Action func in registeredUpdates)
                    func();

            yield return Timing.WaitForOneFrame;
        }
    }

    public void AddFixedUpdate (Action func)
    {
        registeredFixedUpdates.Add(func);
    }

    public void RemoveFixedUpdate (Action func)
    {
        registeredFixedUpdates.Remove(func);
    }

    public IEnumerator<float> CustomFixedUpdate ()
    {
        yield return Timing.WaitForOneFrame;
        while (gameObject != null)
        {
            if (gameObject.activeInHierarchy && enabled)
            {
                OnFixedUpdate();
                foreach (Action func in registeredFixedUpdates)
                    func();
            }
            yield return Timing.WaitForOneFrame;
        }
    }

    public bool TogglePause(bool? pause = null)
    {
        if (pause != null)
            IsPaused = (bool)pause;
        else
            IsPaused = !IsPaused;

        Debug.Log("OnPauseChanged:" + IsPaused);
        OnPauseChanged?.Invoke(IsPaused);
        return IsPaused;
    }

    void OnFixedUpdate()
    {
        if (IsPaused)
            return;

        Physics2D.Simulate(Timing.DeltaTime);
    }

    public void AddLateUpdate (Action func)
    {
        registeredLateUpdates.Add(func);
    }
    
    public void RemoveLateUpdate (Action func)
    {
        registeredLateUpdates.Remove(func);
    }

    public IEnumerator<float> CustomLateUpdate ()
    {
        yield return Timing.WaitForOneFrame;
        while (gameObject != null)
        {
            if (gameObject.activeInHierarchy && enabled)
                foreach (Action func in registeredLateUpdates)
                    func();

            yield return Timing.WaitForOneFrame;
        }
    }

    #endregion

    public void StartNewLevel(int world, int level)
    {
        Timing.KillCoroutines(startLevelCoroutine);
        startLevelCoroutine = Timing.RunCoroutine(LoadLevelCoroutine(world, level));
    }

    public void RestartLevel()
    {
        StartLevel();
    }

    IEnumerator<float> LoadLevelCoroutine (int world, int level)
    {
        if (activeLevel.isLoaded)
            Timing.WaitUntilDone(UnloadLevelAsync());

        yield return Timing.WaitUntilDone(LoadLevelAsync(world, level));

        yield return Timing.WaitForOneFrame;

        Player = Instantiate(Resources.Load<PlayerController>(Constants.PlayerPrefabPath), Vector3.zero, Quaternion.identity);
        Player.OnDeath += OnPlayerDeath;
        Player.GoToSpawnPoint();

        yield return Timing.WaitForOneFrame;

        playerCamera = Instantiate(Resources.Load<PlayerCamera>(Constants.PlayerFollowCameraPrefabPath), Player.transform.position, Quaternion.identity);

        yield return Timing.WaitForOneFrame;

        RotateWorld(0, true);

        yield return Timing.WaitForOneFrame;

        Debug.Log("OnLevelLoadCompleted");
        OnLevelLoadCompleted?.Invoke();

        yield return Timing.WaitForOneFrame;


        canvas = Instantiate(Resources.Load<Canvas>("IngameCanvas"));

        hud = Instantiate(Resources.Load<IngameHUDPanel>("IngameHUDPanel"), canvas.transform);
        levelFailedPanel = Instantiate(Resources.Load<LevelFailedPanel>("LevelFailedPanel"), canvas.transform);
        levelCompletedPanel = Instantiate(Resources.Load<LevelCompletedPanel>("LevelCompletedPanel"), canvas.transform);
        
        StartLevel();
    }

    void StartLevel()
    {
        Player.GoToSpawnPoint();
        RotateWorld(0, true);
        TogglePause(false);
        Debug.Log("OnLevelStarted");
        OnLevelStarted?.Invoke();
    }

    IEnumerator<float> LoadLevelAsync(int world, int level)
    {
        //string sceneName = $"World{world}Level{level}";
        string sceneName = $"LevelScene";

        AsyncOperation loadLevelOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        loadLevelOperation.allowSceneActivation = false;

        while (!loadLevelOperation.isDone)
        {
            if (loadLevelOperation.progress >= 0.9f)// <-- Unity being stupid and finishing loading when progress equals 0.9
                loadLevelOperation.allowSceneActivation = true;

            yield return Timing.WaitForOneFrame;
        }

        activeLevel = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(activeLevel);

        Instantiate(Resources.Load($"World{world}Level{level}"));
        loadedWorldNumber = world;
        loadedLevelNumber = level;
    }

    void UnloadCurrentLevel()
    {
        if(!unloadCoroutine.IsRunning)
            unloadCoroutine = Timing.RunCoroutine(UnloadLevelAsync());
    }

    IEnumerator<float> UnloadLevelAsync()
    {
        if(playerCamera != null)
            Destroy(playerCamera.gameObject);

        if(Player != null)
            Destroy(Player.gameObject);

        AsyncOperation unloadLevelOperation = SceneManager.UnloadSceneAsync(activeLevel);

        while (!unloadLevelOperation.isDone)
            yield return Timing.WaitForOneFrame;

        Resources.UnloadUnusedAssets();

        loadedWorldNumber = 0;
        loadedLevelNumber = 0;

        ApplicationManager.Instance.DestroyGameManager();
    }

    

    public void LevelGoalReached(EndPoint goal)
    {
        Debug.Log("OnLevelGoalReached");
        OnLevelGoalReached?.Invoke(loadedWorldNumber, loadedLevelNumber);

        levelCompletedPanel.Open();
    }
    
    public void LevelFailed()
    {
        Debug.Log("OnLevelFailed");
        OnLevelFailed?.Invoke(loadedWorldNumber, loadedLevelNumber);
        levelFailedPanel.Open();
    }
    
    public void Terminate ()
    {
        Debug.Log("GameManager.Terminate");
        Player.OnDeath -= OnPlayerDeath;
        Destroy(Player);
        Destroy(playerCamera);
        Timing.KillCoroutines(coroutinesTag);
        registeredUpdates.Clear();
        registeredFixedUpdates.Clear();
        registeredLateUpdates.Clear();
        DestroyRotator();

        UnloadCurrentLevel();
    }
}

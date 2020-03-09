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

    PlayerController player;
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
    public event Action<int,int> OnLevelFinished;
    /// <summary>
    /// WorldNumber, LevelNumber
    /// </summary>
    public event Action<int, int> OnLevelFailed;

    const string coroutinesTag = "gameManager";

    public bool IsPaused { get; private set;}

    CoroutineHandle unloadCoroutine;

    private void OnPlayerDeath ()
    {
        Timing.CallDelayed(1, () => StartNewLevel(loadedWorldNumber, loadedLevelNumber));
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

    public bool TogglePause()
    {
        IsPaused = !IsPaused;
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

    IEnumerator<float> LoadLevelCoroutine (int world, int level)
    {
        if (activeLevel.isLoaded)
            Timing.WaitUntilDone(UnloadLevelAsync());

        yield return Timing.WaitUntilDone(LoadLevelAsync(world, level));

        yield return Timing.WaitForOneFrame;

        player = Instantiate(Resources.Load<PlayerController>(Constants.PlayerPrefabPath), Vector3.zero, Quaternion.identity);
        player.OnDeath += OnPlayerDeath;
        player.GoToSpawnPoint();

        yield return Timing.WaitForOneFrame;

        playerCamera = Instantiate(Resources.Load<PlayerCamera>(Constants.PlayerFollowCameraPrefabPath), player.transform.position, Quaternion.identity);

        yield return Timing.WaitForOneFrame;

        RotateWorld(0, true);

        yield return Timing.WaitForOneFrame;

        OnLevelLoadCompleted?.Invoke();

        yield return Timing.WaitForOneFrame;

        OnClickedStarLevel();
    }

    void OnClickedStarLevel()
    {
        OnLevelStarted?.Invoke();
    }

    IEnumerator<float> LoadLevelAsync(int world, int level)
    {
        string sceneName = $"World{world}Level{level}";

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

        if(player != null)
            Destroy(player.gameObject);

        AsyncOperation unloadLevelOperation = SceneManager.UnloadSceneAsync(activeLevel);

        while (!unloadLevelOperation.isDone)
            yield return Timing.WaitForOneFrame;

        Resources.UnloadUnusedAssets();

        loadedWorldNumber = 0;
        loadedLevelNumber = 0;

        Destroy(gameObject);
    }

    

    public void LevelGoalReached(EndPoint goal)
    {
        OnLevelGoalReached?.Invoke(loadedWorldNumber, loadedLevelNumber);

        Timing.KillCoroutines(coroutinesTag);

        Timing.CallDelayed(1, FinishLevel);
    }
    
    public void LevelFailed()
    {
        OnLevelFailed?.Invoke(loadedWorldNumber, loadedLevelNumber);
        Timing.KillCoroutines(coroutinesTag);
    }

    void FinishLevel()
    {
        OnLevelFinished?.Invoke(loadedWorldNumber, loadedLevelNumber);
    }

    public void Terminate ()
    {
        Timing.KillCoroutines(coroutinesTag);
        registeredUpdates.Clear();
        registeredFixedUpdates.Clear();
        registeredLateUpdates.Clear();
        DestroyRotator();

        UnloadCurrentLevel();
    }
}

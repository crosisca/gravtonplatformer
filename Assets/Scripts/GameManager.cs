using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MEC;
using System;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public static GameManager Instance => instance ?? (instance = FindObjectOfType<GameManager>());

    public GameObject activeLevel;

    public int loadedWorldNumber;
    public int loadedLevelNumber;

    PlayerController player;

    CoroutineHandle startLevelCoroutine;

    private void Awake ()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        player.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath ()
    {
        Timing.CallDelayed(1, () => StartNewLevel(loadedWorldNumber, loadedLevelNumber));
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        StartNewLevel(1, 1);
    }

    void Update()
    {
        //Debug load level 1-9 pressing keys
        for (int i = 49; i < 58; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
                StartNewLevel(1,i-48);
        }
    }

    public void StartNewLevel(int world, int level)
    {
        Timing.KillCoroutines(startLevelCoroutine);
        startLevelCoroutine = Timing.RunCoroutine(StartLevelCoroutine(world, level));
    }

    IEnumerator<float> StartLevelCoroutine (int world, int level)
    {
        if(activeLevel != null)
            UnloadCurrentLevel();

        yield return Timing.WaitForOneFrame;

        LoadLevel(world, level);

        yield return Timing.WaitForOneFrame;

        player.GoToSpawnPoint();

        GameRotationManager.Instance.RotateWorld(0, true);

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<PolygonCollider2D>();
    }

    void LoadLevel (int world, int level)
    {
        activeLevel = Instantiate(Resources.Load($"World{world}Level{level}")) as GameObject;

        loadedWorldNumber = world;
        loadedLevelNumber = level;
    }

    public void UnloadCurrentLevel()
    {
        if (activeLevel)
            Destroy(activeLevel);

        loadedWorldNumber = 0;
        loadedLevelNumber = 0;
    }
}

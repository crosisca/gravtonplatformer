using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public static GameManager Instance => instance ?? (instance = FindObjectOfType<GameManager>());

    public GameObject activeLevel;

    public int loadedWorldNumber;
    public int loadedLevelNumber;
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        LoadLevel(1, 1);
    }

    void Update()
    {
        for (int i = 49; i < 58; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
                LoadLevel(1,i-48);
        }
    }

    public void LoadLevel(int world, int level)
    {
        if(activeLevel)
            Destroy(activeLevel);

        activeLevel = Instantiate(Resources.Load($"World{world}Level{level}")) as GameObject;

        loadedWorldNumber = world;
        loadedLevelNumber = level;

        GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<PolygonCollider2D>();
    }
}

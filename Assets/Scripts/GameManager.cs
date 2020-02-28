using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public static GameManager Instance => instance ?? (instance = FindObjectOfType<GameManager>());

    public GameObject activeLevel;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadLevel(1,1);
    }

    void Update()
    {
        for (int i = 49; i < 58; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
                LoadLevel(1,i-48);
        }
    }

    void LoadLevel(int world, int level)
    {
        if(activeLevel)
            Destroy(activeLevel);

        activeLevel = Instantiate(Resources.Load($"World{world}Level{level}")) as GameObject;
        GameObject.FindGameObjectWithTag("Player").transform.position = Vector3.zero;
    }
}

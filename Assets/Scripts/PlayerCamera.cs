using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    CinemachineConfiner confiner;

    GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner>();
        GameManager.Instance.OnLevelLoadCompleted += OnLevelLoadCompleted;
        GameManager.Instance.OnLevelCompleted += OnLevelCompleted;
    }

    void OnLevelLoadCompleted ()
    {
        confiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<PolygonCollider2D>();
        virtualCamera.Follow = GameObject.FindWithTag("Player").transform;

        player = GameObject.FindWithTag("Player");
        GameManager.Instance.AddLateUpdate(OnLateUpdate);
    }
    
    void OnLateUpdate()
    {
        if (!Application.isMobilePlatform)
            transform.rotation = player.transform.rotation;
    }

    void OnLevelCompleted (int worldNumber, int levelNumber)
    {
        GameManager.Instance.RemoveLateUpdate(OnLateUpdate);
        GameManager.Instance.OnLevelLoadCompleted -= OnLevelLoadCompleted;
        GameManager.Instance.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnDestroy ()
    {
        GameManager.Instance.RemoveLateUpdate(OnLateUpdate);
    }
}

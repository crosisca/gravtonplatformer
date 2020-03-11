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
        GameManager.Instance.OnLevelStarted += OnLevelStarted;
        GameManager.Instance.OnLevelFailed += OnLevelFailed;
        GameManager.Instance.OnLevelGoalReached += OnLevelGoalReached;
        GameManager.Instance.OnLevelLoadCompleted += OnLevelLoadCompleted;
        GameManager.Instance.OnLevelFinished += OnLevelFinished;
    }
    
    void OnLevelLoadCompleted ()
    {
        confiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<PolygonCollider2D>();
        virtualCamera.Follow = GameObject.FindWithTag("Player").transform;

        player = GameObject.FindWithTag("Player");
    }
    
    void OnLevelStarted ()
    {
        GameManager.Instance.AddLateUpdate(OnLateUpdate);
    }

    void OnLevelGoalReached (int arg1, int arg2)
    {
        GameManager.Instance.RemoveLateUpdate(OnLateUpdate);
    }

    void OnLevelFailed (int arg1, int arg2)
    {
        GameManager.Instance.RemoveLateUpdate(OnLateUpdate);
    }

    void OnLateUpdate()
    {
        if (!Application.isMobilePlatform)
            transform.rotation = player.transform.rotation;
    }

    void OnLevelFinished (int worldNumber, int levelNumber)
    {
        GameManager.Instance.RemoveLateUpdate(OnLateUpdate);
    }

    private void OnDestroy ()
    {
        GameManager.Instance.OnLevelStarted -= OnLevelStarted;
        GameManager.Instance.OnLevelFailed -= OnLevelFailed;
        GameManager.Instance.OnLevelGoalReached -= OnLevelGoalReached;
        GameManager.Instance.OnLevelLoadCompleted -= OnLevelLoadCompleted;
        GameManager.Instance.OnLevelFinished -= OnLevelFinished;
        GameManager.Instance.RemoveLateUpdate(OnLateUpdate);
    }
}

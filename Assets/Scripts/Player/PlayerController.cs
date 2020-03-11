using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class PlayerController : MonoBehaviour
{
    public event Action OnDeath;
    //public event Action OnDeathCompleted;

    public PlayerMovement Movement { get; private set; }

    [SerializeField]
    float LandSpeedLimit = 15;

    private void Awake ()
    {
        Movement = GetComponent<PlayerMovement>();
        Movement.OnLand += OnLand;

        LandSpeedLimit = Mathf.Abs(Physics2D.gravity.y);
    }

    public void GoToSpawnPoint()
    {
        transform.position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
    }

    void OnLand ()
    {
        if (Movement.DownwardsVelocity > LandSpeedLimit)
        {
            Debug.Log("Death by FALL. speed: " + Movement.DownwardsVelocity);
            Kill(DeathReason.HIGH_FALL);
        }
    }

    public void Kill(DeathReason reason = DeathReason.DEFAULT)
    {
        //Play death animation
        switch (reason)
        {
            case DeathReason.DEFAULT:
                break;
            case DeathReason.FIRE:
                break;
            case DeathReason.SHOCK:
                break;
        }
        OnDeath?.Invoke();
    }

    private void OnDestroy ()
    {
        Movement.OnLand -= OnLand;
    }
}

public enum DeathReason
{
    DEFAULT,
    FIRE,
    SHOCK,
    HIGH_FALL,
}

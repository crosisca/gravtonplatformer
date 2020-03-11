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

    private void Awake ()
    {
        Movement = GetComponent<PlayerMovement>();
    }

    public void GoToSpawnPoint()
    {
        transform.position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
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
}

public enum DeathReason
{
    DEFAULT,
    FIRE,
    SHOCK,
}

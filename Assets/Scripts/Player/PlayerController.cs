using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class PlayerController : MonoBehaviour
{
    public Action OnDeath;
    public Action OnDeathCompleted;

    public PlayerInput input;

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

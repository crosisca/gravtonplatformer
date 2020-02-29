using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class PlayerController : MonoBehaviour
{
    public Action OnDeath;
    public Action OnDeathCompleted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

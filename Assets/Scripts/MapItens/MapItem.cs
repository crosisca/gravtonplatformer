using System;
using UnityEngine;

public class MapItem : MonoBehaviour
{
    public bool Active { get;private set; } = false;

    protected Vector2 Position2d => transform.position.AsVector2();

    public virtual void Awake () { }

    public virtual void Start () 
    {
        GameManager.Instance.OnLevelStarted += OnLevelStarted;
        GameManager.Instance.OnLevelFailed += OnLevelFailed;
        GameManager.Instance.OnLevelGoalReached += OnLevelGoalReached;
    }

    protected virtual void OnLevelStarted ()
    {
        Activate();
    }

    void OnLevelFailed (int arg1, int arg2)
    {
        Deactivate();
    }

    void OnLevelGoalReached(int arg1, int arg2)
    {
        Deactivate();
    }

    void Activate()
    {
        Active = true;

        GameManager.Instance.AddUpdate(OnUpdate);
        GameManager.Instance.AddFixedUpdate(OnFixedUpdate);
    }
    
    void Deactivate()
    {
        Active = false;

        GameManager.Instance.RemoveUpdate(OnUpdate);
        GameManager.Instance.RemoveFixedUpdate(OnFixedUpdate);
    }

    protected virtual void OnUpdate()
    {
        if (!Active || GameManager.Instance.IsPaused)
            return;
    }

    protected virtual void OnFixedUpdate()
    {
        if (!Active || GameManager.Instance.IsPaused) 
            return;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Active)
            return;
    }

    protected virtual void OnTriggerEnter2D (Collider2D collision)
    {
        if (!Active) 
            return;
    }

    void OnDestroy ()
    {
        GameManager.Instance.OnLevelStarted -= OnLevelStarted;
        GameManager.Instance.OnLevelFailed -= OnLevelFailed;
        GameManager.Instance.OnLevelGoalReached -= OnLevelGoalReached;
    }
}

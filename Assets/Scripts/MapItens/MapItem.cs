using System;
using UnityEngine;

public class MapItem : MonoBehaviour
{
    [SerializeField]
    public bool startActivated = true;

    public bool Active { get;private set; } = false;
    
    protected virtual void Awake () { }

    public virtual void Start () 
    {
        GameManager.Instance.OnLevelStarted += OnLevelStarted;
        GameManager.Instance.OnLevelFailed += OnLevelFailed;
        GameManager.Instance.OnLevelGoalReached += OnLevelGoalReached;
    }

    protected virtual void OnLevelStarted ()
    {
        if(startActivated)
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

    public virtual void Activate ()
    {
        if (Active)
            return;

        Active = true;

        GameManager.Instance.AddUpdate(OnUpdate);
        GameManager.Instance.AddFixedUpdate(OnFixedUpdate);
    }
    
    protected virtual void Deactivate()
    {
        if (!Active)
            return;

        Active = false;

        GameManager.Instance.RemoveUpdate(OnUpdate);
        GameManager.Instance.RemoveFixedUpdate(OnFixedUpdate);
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnFixedUpdate()
    {
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
    }

    protected virtual void OnTriggerEnter2D (Collider2D collider)
    {
    }

    protected virtual void OnDestroy ()
    {
        GameManager.Instance.OnLevelStarted -= OnLevelStarted;
        GameManager.Instance.OnLevelFailed -= OnLevelFailed;
        GameManager.Instance.OnLevelGoalReached -= OnLevelGoalReached;
    }
}

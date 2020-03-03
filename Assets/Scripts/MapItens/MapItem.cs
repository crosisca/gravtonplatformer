using UnityEngine;

public class MapItem : MonoBehaviour
{
    public bool Active { get;private set; } = false;

    protected virtual void OnLevelStarted()
    {
        Active = true;

        GameManager.Instance.AddUpdate(OnUpdate);
        GameManager.Instance.AddFixedUpdate(OnFixedUpdate);
    }

    protected virtual void OnUpdate()
    {
        if (!Active)
            return;
    }

    protected virtual void OnFixedUpdate()
    {
        if (!Active) 
            return;
    }

    protected virtual void OnTriggerEnter2D (Collider2D collision)
    {
        if (!Active) 
            return;
    }

    protected virtual void OnLevelGoalReached ()
    {
        GameManager.Instance.RemoveUpdate(OnUpdate);
        GameManager.Instance.RemoveFixedUpdate(OnFixedUpdate);

        Active = false;
    }

    protected virtual void OnLevelCompleted()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MapItem
{
    protected override void OnTriggerEnter2D (Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        if (collider.CompareTag("Player"))
            GameManager.Instance.LevelGoalReached(this);
    }
}

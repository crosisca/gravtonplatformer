using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKillHazard : MapItem
{
    protected virtual DeathReason DeathReason => DeathReason.DEFAULT;

    protected override void OnCollisionEnter2D (Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
            GameManager.Instance.Player.Kill(DeathReason);
    }

    protected override void OnTriggerEnter2D (Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        if (collider.CompareTag("Player"))
            GameManager.Instance.Player.Kill(DeathReason);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKillHazard : MapItem
{
    protected virtual DeathReason DeathReason => DeathReason.DEFAULT;
    protected override void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            GameManager.Instance.Player.Kill(DeathReason);
    }
}

using Lean.Pool;
using UnityEngine;

public class Stalactite : InstantKillHazard
{
    [SerializeField]
    float delayBetweenDrops = 3;

    float timer;

    [SerializeField]
    StalactiteProjectile projectilePrefab;

    [SerializeField]
    float dropProjectileSpeed = 5;

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        timer += Time.fixedDeltaTime;

        if (timer >= delayBetweenDrops)
        {
            timer = 0;
            StalactiteProjectile projectile = LeanPool.Spawn(projectilePrefab, transform.position, transform.rotation);
            projectile.Setup(dropProjectileSpeed);
        }
    }
}
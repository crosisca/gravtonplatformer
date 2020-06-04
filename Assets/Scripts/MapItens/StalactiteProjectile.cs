using Lean.Pool;
using UnityEngine;

public class StalactiteProjectile : InstantKillHazard, IPoolable
{
    float moveSpeed;

    new Rigidbody2D rigidbody2D;

    protected override void Awake()
    {
        base.Awake();

        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.isKinematic = true;
    }

    public void OnSpawn()
    {
        Activate();
    }
    
    public void Setup (float dropSpeed)
    {
        moveSpeed = dropSpeed;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        rigidbody2D.MovePosition(rigidbody2D.position + Vector2.down.Rotate(rigidbody2D.rotation) * moveSpeed * Time.fixedDeltaTime);
    }

    //void FixedUpdate ()
    //{
    //    rigidbody2D.MovePosition(rigidbody2D.position + Vector2.down.Rotate(rigidbody2D.rotation) * moveSpeed * Time.fixedDeltaTime);
    //}

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        Debug.Log($"Collided with {collision.gameObject.name}");

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Player") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
            Despawn();
    }
    
    void Despawn()
    {
        LeanPool.Despawn(gameObject);
    }

    public void OnDespawn()
    {
        Deactivate();
    }

    
}
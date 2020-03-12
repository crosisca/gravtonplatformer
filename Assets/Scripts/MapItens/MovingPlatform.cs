using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class MovingPlatform : MapItem
{
    Vector2 startPosition;

    [SerializeField]
    Transform endPositionTransform;

    Vector2 endPosition;

    [SerializeField]
    float speed = 1;

    int direction = 1;

    Rigidbody2D rb;
    Vector2 forwardDirection;

    List<Rigidbody2D> collidingObjects = new List<Rigidbody2D>();

    public override void Awake()
    {
        base.Awake();

        startPosition = Position2d;

        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        endPosition = endPositionTransform.position.AsVector2();
        forwardDirection = (endPosition - startPosition).normalized;

        direction = 1;
    }

    protected override void OnLevelStarted ()
    {
        base.OnLevelStarted();

        rb.position = startPosition;
        rb.velocity = Vector2.zero;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        Vector2 targetPosition = direction == 1 ? endPosition : startPosition;

        //rb.MovePosition(Vector2.MoveTowards(Position2d, targetPosition, speed * Time.fixedDeltaTime));
        rb.velocity = (targetPosition - Position2d).normalized * speed;

        if (Vector2.Distance(transform.position.AsVector2(), targetPosition) < speed * Time.fixedDeltaTime)
            direction = -direction;

        for (int i = 0; i < collidingObjects.Count; i++)
        {
            Debug.Log("Essa ideia parece funcionar porem o player esta resetando (usar modifier stacks?)");
            collidingObjects[i].velocity += rb.velocity;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (180 - Mathf.Abs(Vector3.Angle(transform.up, collision.contacts[0].normal)) < 90)
            collidingObjects.Add(collision.rigidbody);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collidingObjects.Remove(collision.rigidbody);
    }
}

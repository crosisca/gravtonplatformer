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
        
        rb.MovePosition(Vector2.MoveTowards(Position2d, targetPosition, speed * Time.fixedDeltaTime));

        if (Vector2.Distance(transform.position.AsVector2(), targetPosition) < speed * Time.fixedDeltaTime)
            direction = -direction;
    }
}

using System.Collections.Generic;
using MEC;
using UnityEngine;

public class MovingDoor : MapItem
{
    new Rigidbody2D rigidbody2D;

    [SerializeField]
    float yRotationWhenOpen;

    [SerializeField]
    float rotationSpeed = 90;

    float originalRotation;

    CoroutineHandle openDoorCoroutine;

    protected override void Awake()
    {
        base.Awake();

        rigidbody2D = GetComponentInChildren<Rigidbody2D>();

        originalRotation = rigidbody2D.rotation;
    }

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        rigidbody2D.rotation = originalRotation;
        
        Timing.KillCoroutines(openDoorCoroutine);
    }

    public override void Activate ()
    {
        base.Activate();

        openDoorCoroutine = Timing.RunCoroutine(OpenDoorCoroutine(), Segment.FixedUpdate);
    }

    IEnumerator<float> OpenDoorCoroutine()
    {
        float angleDif = MathfUtils.Clamp0360(yRotationWhenOpen - rigidbody2D.rotation);
        int sign = angleDif > 0 && angleDif < 180 ? 1 : -1;

        while (Mathf.Abs(MathfUtils.Clamp0360(rigidbody2D.rotation) - MathfUtils.Clamp0360(yRotationWhenOpen)) > rotationSpeed * Time.fixedDeltaTime)
        {
            Debug.Log($"opening {MathfUtils.Clamp0360(rigidbody2D.rotation)} | {MathfUtils.Clamp0360(yRotationWhenOpen)}");
            //rigidbody2D.MoveRotation(rotationSpeed * Time.fixedDeltaTime);
            rigidbody2D.MoveRotation(rigidbody2D.rotation + rotationSpeed * sign * Time.fixedDeltaTime);
            yield return Timing.WaitForOneFrame;
        }
        rigidbody2D.MoveRotation(yRotationWhenOpen);
    }
    
    protected override void Deactivate()
    {
        base.Deactivate();
        Timing.KillCoroutines(openDoorCoroutine);
    }

    
}
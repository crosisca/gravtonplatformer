﻿using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class GameRotationManager : MonoBehaviour
{
    static GameRotationManager instance;

    public static GameRotationManager Instance => instance ?? (instance = FindObjectOfType<GameRotationManager>());

    [Header("References")]
    public PlayerMovement player;
    public Transform currentCameraTransform;

    [Header("Debug")]
    public bool rotating;

    public int currentAngle = 0;

    Vector2 desiredGravity;
    float gravityMagnitude;
    public float worldRotationSpeed = 10;

    public Vector2 DirectionToGround => Vector2.down.Rotate(currentAngle);

    public GravityDirection GravityDirection
    {
        get
        {
            switch (currentAngle)
            {
                case 0:
                    return GravityDirection.DOWN;
                case 90:
                    return GravityDirection.RIGHT;
                case 180:
                    return GravityDirection.UP;
                case 270:
                    return GravityDirection.LEFT;
            }

            return GravityDirection.DOWN;
        }
    }

    // Start is called before the first frame update
    void Start ()
    {
        desiredGravity = Physics2D.gravity;
        gravityMagnitude = Physics2D.gravity.magnitude;
    }

    // Update is called once per frame
    void Update ()
    {
        CheckRotationInputs();
    }

    void CheckRotationInputs ()
    {
        if (rotating)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            RotateWorld(currentAngle + 90);
        else if (Input.GetKeyDown(KeyCode.Q))
            RotateWorld(currentAngle - 90);
    }

    private void FixedUpdate ()
    {
        desiredGravity = rotating ? Vector2.zero : (Vector2.down * gravityMagnitude).Rotate(currentAngle);

        if (Physics2D.gravity != desiredGravity)
            Physics2D.gravity = desiredGravity;
    }

    public void RotateWorld (int targetAngle, bool instant = false)
    {
        targetAngle = Clamp0360(targetAngle);
        if(instant)
        {
            currentCameraTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
            player.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            currentAngle = Clamp0360(targetAngle);
        }
        else
            Timing.RunCoroutine(RotateWorldCoroutine(targetAngle));
    }
    
    IEnumerator<float> RotateWorldCoroutine(int targetAngle)
    {
        rotating = true;

        Physics2D.gravity = Vector2.zero;

        targetAngle = Clamp0360(targetAngle);
        int initialAngle = currentAngle;
        float t = 0;
        while (t < 1)
        {
            t += Timing.DeltaTime * worldRotationSpeed;
            int lerpAngle = Clamp0360((int)Mathf.LerpAngle(initialAngle, targetAngle, t));

            currentCameraTransform.eulerAngles = new Vector3(0, 0, lerpAngle);
            player.transform.eulerAngles = new Vector3(0, 0, lerpAngle);

            yield return Timing.WaitForOneFrame;
        }

        rotating = false;
        currentCameraTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
        player.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        currentAngle = Clamp0360(targetAngle);
    }

    public static int Clamp0360 (int eulerAngles)
    {
        int result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360) * 360;
        if (result < 0)
        {
            result += 360;
        }
        return result;
    }

    void OnDrawGizmos ()
    {
        if (!Application.isPlaying)
            return;

        Color originalColor = Gizmos.color;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(player.transform.position, player.transform.position + new Vector3(Physics2D.gravity.x, Physics2D.gravity.y, 0).normalized);
        Gizmos.color = originalColor;
    }
}

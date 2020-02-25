using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRotationManager : MonoBehaviour
{
    static GameRotationManager instance;

    public static GameRotationManager Instance => instance ?? (instance = FindObjectOfType<GameRotationManager>());

    [Header("Settings")]
    public bool disablePlayerColliderWhileRotating = false;

    public PlayerController player;

    public bool rotating;

    float gravityForce;

    public int currentAngle = 0;

    public Transform currentCameraTransform;
    Vector2 desiredGravity;

    public Vector2 DirectionToGround => Vector2.down.Rotate(currentAngle);

    // Start is called before the first frame update
    void Start ()
    {
        desiredGravity = Physics2D.gravity;
        gravityForce = Physics2D.gravity.magnitude;
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
        {
            currentAngle = Clamp0360(currentAngle + 90);
            StartCoroutine(RotateCamera(90));
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            currentAngle = Clamp0360(currentAngle - 90);
            StartCoroutine(RotateCamera(-90));
        }

        desiredGravity = rotating ? Vector2.zero : (Vector2.down * gravityForce).Rotate(currentAngle);

        if (Physics2D.gravity != desiredGravity)
            Physics2D.gravity = desiredGravity;
    }

    IEnumerator RotateCamera (float angle)
    {
        rotating = true;

        Physics2D.gravity = Vector2.zero;
        
        if(disablePlayerColliderWhileRotating)
            player.Collider.enabled = false;
        
        for (int i = 0; i < Mathf.Abs(angle); i++)
        {
            currentCameraTransform.Rotate(Vector3.forward, Mathf.Sign(angle));
            player.transform.RotateAround(player.Visual.position, Vector3.forward, Mathf.Sign(angle));
            yield return null;
        }

        if (disablePlayerColliderWhileRotating)
            player.Collider.enabled = true;
        
        rotating = false;
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
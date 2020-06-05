using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

public class PlayerController : MonoBehaviour
{
    public event Action OnDeath;

    public Vector2 LocalMoveVector => localMoveVector;

    [SerializeField]//debug only
    Vector2 localMoveVector;

    [SerializeField]
    float gravity = 50;

    [SerializeField]
    float maxHorizontalSpeed = 50;

    [SerializeField]
    float jumpSpeed = 10;

    [SerializeField]
    float acceleration = 500;
    [SerializeField]
    float deceleration = 500;

    [SerializeField]
    float LandSpeedLimit = 20;

    [SerializeField]
    float MaxFallSpeed = 25;

    [SerializeField]
    float slidingAcceleration = 5;
    [SerializeField]
    float slidingDeceleration = 0;

    PlayerInput input;
    CharacterController2D characterController;
    public CharacterController2D CharacterController => characterController;

    bool movementEnabled;

    public bool canMove = true;

    public bool isSliding = false;


    protected const float k_GroundedStickingVelocityMultiplier = 3f;    // This is to help the character stick to vertically moving platforms.

    void Awake ()
    {
        input = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController2D>();
        characterController.OnLand += OnLand;
    }

    void Start()
    {
        GameManager.Instance.OnLevelStarted += OnLevelStarted;
        GameManager.Instance.OnLevelGoalReached += OnGoalreached;
        GameManager.Instance.OnLevelFailed += OnLevelFailed;
    }

    void OnLevelStarted ()
    {
        ToggleMovement(true);
    }

    void OnGoalreached (int arg1, int arg2)
    {
        ToggleMovement(false);
    }

    void OnLevelFailed (int arg1, int arg2)
    {
        ToggleMovement(false);
    }

    void ToggleMovement (bool enabled)
    {
        if (movementEnabled == enabled)
            return;

        movementEnabled = enabled;
        
        if (movementEnabled)
            GameManager.Instance.AddFixedUpdate(OnFixedUpdate);
        else
            GameManager.Instance.RemoveFixedUpdate(OnFixedUpdate);
    }

    void OnFixedUpdate ()
    {
        if (GameManager.Instance.IsPaused)
            return;

        if (GameManager.Instance.IsRotating)
        {
            localMoveVector = Vector2.zero;
            return;
        }

        if (canMove)
        {
            CheckHorizontalMovement(true);
            CheckVerticalMovement();
        }

        CheckJump();

        Vector2 worldRelativeMoveVector = localMoveVector.Rotate(GameManager.Instance.WorldRotationAngle);

        characterController.Move(worldRelativeMoveVector * Time.fixedDeltaTime);
    }

    public void CheckHorizontalMovement (bool useInput, float speedScale = 1f)
    {
        float desiredSpeed = useInput ? input.horizontal * maxHorizontalSpeed * speedScale : 0f;
        float tempAcceleration = useInput && Mathf.Approximately(input.horizontal, 0) ? isSliding ? slidingDeceleration : deceleration : isSliding ? slidingAcceleration : acceleration;
        
        localMoveVector.x = Mathf.MoveTowards(localMoveVector.x, desiredSpeed, tempAcceleration * Time.fixedDeltaTime);

        //Remove lateral movement if blocked by a wall
        if (localMoveVector.x > 0 && characterController.IsBlockedOnRight || 
            localMoveVector.x < 0 && characterController.IsBlockedOnLeft)
            localMoveVector.x = 0;
    }

    public void CheckVerticalMovement ()
    {
        //Check top hit and stop moving up
        if (Mathf.Approximately(localMoveVector.y, 0f) ||
            characterController.IsCeilinged && localMoveVector.y > 0f ||
            characterController.IsGrounded && localMoveVector.y < -gravity * Time.fixedDeltaTime)
        {
            localMoveVector.y = 0f;
        }
        
        //if(!characterController.IsGrounded) //Not necessary but avoids moving vector being always negative on Y
        localMoveVector.y -= gravity * Time.fixedDeltaTime;
        
        if (localMoveVector.y < -MaxFallSpeed)
        {
            localMoveVector.y = -MaxFallSpeed;
        }
    }
    
    public void CheckJump ()
    {
        if (input.jumpPressed && characterController.IsGrounded)
        {
            localMoveVector.y = jumpSpeed;
        }
    }

    public void GoToSpawnPoint()
    {
        transform.position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
    }

    void OnLand ()
    {
        Debug.Log($"Landed with speed of {characterController.DownwardsVelocity}");
        if (characterController.DownwardsVelocity > LandSpeedLimit)
        {
            Debug.Log($"Death by FALL. LandSpeedLimit: {LandSpeedLimit}");
            Kill(DeathReason.HIGH_FALL);
        }
    }

    public void Kill(DeathReason reason = DeathReason.DEFAULT)
    {
        //Play death animation
        switch (reason)
        {
            case DeathReason.DEFAULT:
                break;
            case DeathReason.FIRE:
                break;
            case DeathReason.SHOCK:
                break;
        }
        OnDeath?.Invoke();
    }
    
     void OnDestroy ()
    {
        characterController.OnLand -= OnLand;
        GameManager.Instance.OnLevelStarted -= OnLevelStarted;
        GameManager.Instance.OnLevelFailed -= OnLevelFailed;
        GameManager.Instance.OnLevelGoalReached -= OnGoalreached;
        GameManager.Instance.RemoveFixedUpdate(OnFixedUpdate);
    }

    void Update()
    {
        //Teport to mouse
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            characterController.Teleport(pos);
        }
    }
}

public enum DeathReason
{
    DEFAULT,
    FIRE,
    SHOCK,
    HIGH_FALL,
}

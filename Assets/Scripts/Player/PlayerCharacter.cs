using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    Vector2 localMoveVector;

    PlayerInput input;

    [SerializeField]
    float gravity = 50;

    [SerializeField]
    float maxHorizontalSpeed = 50;

    [SerializeField]
    float jumpSpeed = 10;

    public float acceleration = 100f;
    public float deceleration = 100f;

    CharacterController2D characterController;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController2D>();
    }
    
    void FixedUpdate ()
    {
        UpdateHorizontalMovement(true);
        UpdateVerticalMovement();
        UpdateJump();

        Vector2 worldRelativeMoveVector = localMoveVector.Rotate(GameManager.Instance.WorldRotationAngle);

        characterController.Move(worldRelativeMoveVector * Time.fixedDeltaTime);
    }

    public void UpdateHorizontalMovement (bool useInput, float speedScale = 1f)
    {
        float desiredSpeed = useInput ? input.horizontal * maxHorizontalSpeed * speedScale : 0f;
        float tempAcceleration = useInput && Mathf.Approximately(input.horizontal, 0) ? acceleration : deceleration;
        localMoveVector.x = Mathf.MoveTowards(localMoveVector.x, desiredSpeed, tempAcceleration * Time.deltaTime);
    }

    public void UpdateVerticalMovement ()
    {
        localMoveVector.y -= gravity * Time.deltaTime;

        //Check top hit and stop moving up
        if (Mathf.Approximately(localMoveVector.y, 0f) || characterController.IsCeilinged && localMoveVector.y > 0f)
            localMoveVector.y = 0f;
    }

    public void UpdateJump ()
    {
        if (input.jumpPressed && characterController.IsGrounded)
        {
            localMoveVector.y = jumpSpeed;
        }
    }
}
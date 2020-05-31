//using System;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerMovement : MonoBehaviour
//{
//    //public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

//    //[Header("Movement Properties")]
//    //public float speed = 5f;                //Player speed
//    //public float coyoteDuration = .05f;     //How long the player can jump after falling
//    //public float maxFallSpeed = 25f;       //Max speed player can fall

//    //[Header("Jump Properties")]
//    //public float jumpForce = 6.3f;          //Initial force of jump
//    ////public float jumpHoldForce = 1.9f;      //Incremental force when jump is held
//    ////public float jumpHoldDuration = .1f;    //How long the jump key can be held

//    //[Header("Environment Check Properties")]
//    //public float footOffset = .4f;          //X Offset of feet raycast
//    //public float eyeHeight = 1.5f;          //Height of wall checks
//    //public float reachOffset = .7f;         //X offset for wall grabbing
//    //public float groundDistance = .2f;      //Distance player is considered to be on the ground
//    //public LayerMask groundLayer;           //Layer of the ground

//    //[Header("Status Flags")]
//    //public bool isOnGround;                 //Is the player on the ground?
//    ////public bool isJumping;                  //Is player jumping?

//    //PlayerInput input;                      //The current inputs for the player
//    //Rigidbody2D rigidBody;                  //The rigidbody component

//    //float jumpTime;                         //Variable to hold jump duration
//    //float coyoteTime;                       //Variable to hold coyote duration

//    float originalXScale;                   //Original scale on X axis
//    int direction = 1;                      //Direction player is facing

//    //public event Action OnLand;

//    //public BoxCollider2D Collider { get; private set; }

//    public Transform Visual { get; private set; }

//    bool movementEnabled;

//    //public float DownwardsVelocity
//    //{
//    //    get
//    //    {
//    //        switch (GameManager.Instance.GravityDirection)
//    //        {
//    //            case GravityDirection.UP:
//    //                return rigidBody.velocity.y;
//    //            case GravityDirection.DOWN:
//    //                return -rigidBody.velocity.y;
//    //            case GravityDirection.RIGHT:
//    //                return rigidBody.velocity.x;
//    //            case GravityDirection.LEFT:
//    //                return -rigidBody.velocity.x;
//    //            default:
//    //                return 0;
//    //        }
//    //    }
//    //}

//    //public float LateralVelocity
//    //{
//    //    get
//    //    {
//    //        switch (GameManager.Instance.GravityDirection)
//    //        {
//    //            case GravityDirection.UP:
//    //                return -rigidBody.velocity.x;
//    //            case GravityDirection.DOWN:
//    //                return rigidBody.velocity.x;
//    //            case GravityDirection.RIGHT:
//    //                return rigidBody.velocity.y;
//    //            case GravityDirection.LEFT:
//    //                return -rigidBody.velocity.y;
//    //            default:
//    //                return 0;
//    //        }
//    //    }
//    //}


//    [SerializeField]
//    float fallingSpeed;
//    [SerializeField]
//    GravityDirection gravityDirection;
//    //private void Awake()
//    //{
//    //    groundLayer = LayerMask.GetMask("Ground","Platform");
//    //}

//    void Start ()
//    {
//        //Get a reference to the required components
//        input = GetComponent<PlayerInput>();
//        rigidBody = GetComponent<Rigidbody2D>();
//        Collider = GetComponent<BoxCollider2D>();

//        //Record the original x scale of the player
//        originalXScale = transform.localScale.x;

//        Visual = transform.Find("Gravton_Character");

//        GameManager.Instance.OnLevelStarted += OnLevelStarted;
//        GameManager.Instance.OnLevelGoalReached += OnGoalreached;
//        GameManager.Instance.OnLevelFailed += OnLevelFailed;
//    }

//    void ToggleMovement(bool enabled)
//    {
//        if (movementEnabled == enabled)
//            return;

//        movementEnabled = enabled;

//        rigidBody.isKinematic = !movementEnabled;
//        if (movementEnabled)
//            GameManager.Instance.AddFixedUpdate(OnFixedUpdate);
//        else
//            GameManager.Instance.RemoveFixedUpdate(OnFixedUpdate);
//    }

//    void OnLevelStarted()
//    {
//        ToggleMovement(true);
//    }

//    void OnGoalreached (int arg1, int arg2)
//    {
//        ToggleMovement(false);
//    }

//    void OnLevelFailed (int arg1, int arg2)
//    {
//        ToggleMovement(false);
//    }
    
//    void OnFixedUpdate ()
//    {
//        if (GameManager.Instance.IsPaused)
//            return;

//        //Check the environment to determine status
//        PhysicsCheck();

//        //Process ground and air movements
//        HorizontalMovement();
//        VerticalMovement();

//        UpdateDebugVars();

//        //Limit horizontal velocity
//        //LimitLateralVelocity()
//    }

//    //void LimitLateralVelocity()
//    //{
//    //    Debug.Log(LateralVelocity);
//    //    float maxLateralVelocity = 5;
//    //    if (Mathf.Abs(LateralVelocity) > maxLateralVelocity)
//    //    {
//    //        switch (GameManager.Instance.GravityDirection)
//    //        {
//    //            case GravityDirection.DOWN:
//    //                rigidBody.velocity = new Vector2(maxLateralVelocity * Mathf.Sign(LateralVelocity), rigidBody.velocity.y);
//    //                break;
//    //            case GravityDirection.UP:
//    //                rigidBody.velocity = new Vector2(-maxLateralVelocity * Mathf.Sign(LateralVelocity), rigidBody.velocity.y);
//    //                break;
//    //            case GravityDirection.RIGHT:
//    //                rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxLateralVelocity * Mathf.Sign(LateralVelocity));
//    //                break;
//    //            case GravityDirection.LEFT:
//    //                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -maxLateralVelocity * Mathf.Sign(LateralVelocity));
//    //                break;
//    //        }
//    //    }
//    //}

//    void UpdateDebugVars()
//    {
//        fallingSpeed = DownwardsVelocity;
//        gravityDirection = GameManager.Instance.GravityDirection;
//    }

//    bool wasOnGround;

//    //void PhysicsCheck ()
//    //{
//    //    //Start by assuming the player isn't on the ground and the head isn't blocked
//    //    //isOnGround = false;

//    //    //Cast rays for the left and right foot
//    //    //RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f).Rotate(GameManager.Instance.WorldRotationAngle), GameManager.Instance.DirectionToGround, groundDistance);
//    //    //RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f).Rotate(GameManager.Instance.WorldRotationAngle), GameManager.Instance.DirectionToGround, groundDistance);

//    //    isOnGround = leftCheck || rightCheck;

//    //    if (isOnGround && !wasOnGround)
//    //        OnLand?.Invoke();

//    //    wasOnGround = isOnGround;

//    //    //If either ray hit the ground, the player is on the ground
//    //    //if (leftCheck || rightCheck)
//    //    //{
//    //    //    isOnGround = true;
//    //    //}
//    //}

//    void HorizontalMovement ()
//    {
//        //Calculate the desired velocity based on inputs
//        Debug.Log("a linha abaixo reseta a speed aumentada pela plataforma(usar modifier stacks?)");
//        float horizontalVelocity = speed * input.horizontal;

//        //If the sign of the velocity and direction don't match, flip the character
//        if (horizontalVelocity * direction < 0f)
//            FlipCharacterDirection();

//        if (GameManager.Instance.IsRotating)
//        {
//            rigidBody.velocity = Vector2.zero;
//            return;
//        }

//        SetLocalHorizontalVelocity(horizontalVelocity);

//        //If the player is on the ground, extend the coyote time window
//        if (isOnGround)
//            coyoteTime = Time.time + coyoteDuration;
//    }

//    //void SetLocalHorizontalVelocity(float horizontalVel)
//    //{
//    //    switch (GameManager.Instance.GravityDirection)
//    //    {
//    //        case GravityDirection.DOWN:
//    //            //rigidBody.AddForce(new Vector2(horizontalVel, 0), ForceMode2D.Impulse);
//    //            rigidBody.velocity = new Vector2(horizontalVel, rigidBody.velocity.y);
//    //            break;
//    //        case GravityDirection.UP:
//    //            //rigidBody.AddForce(new Vector2(-horizontalVel, 0), ForceMode2D.Impulse);
//    //            rigidBody.velocity = new Vector2(-horizontalVel, rigidBody.velocity.y);
//    //            break;
//    //        case GravityDirection.RIGHT:
//    //            //rigidBody.AddForce(new Vector2(0, horizontalVel), ForceMode2D.Impulse);
//    //            rigidBody.velocity = new Vector2(rigidBody.velocity.x, horizontalVel);
//    //            break;
//    //        case GravityDirection.LEFT:
//    //            //rigidBody.AddForce(new Vector2(0, -horizontalVel), ForceMode2D.Impulse);
//    //            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -horizontalVel);
//    //            break;
//    //    }
//    //}

//    //void SetLocalVerticalVelocity(float verticalVel)
//    //{
//    //    switch (GameManager.Instance.GravityDirection)
//    //    {
//    //        case GravityDirection.DOWN:
//    //            rigidBody.velocity = new Vector2(rigidBody.velocity.x, verticalVel);
//    //            break;
//    //        case GravityDirection.UP:
//    //            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -verticalVel);
//    //            break;
//    //        case GravityDirection.RIGHT:
//    //            rigidBody.velocity = new Vector2(-verticalVel, rigidBody.velocity.y);
//    //            break;
//    //        case GravityDirection.LEFT:
//    //            rigidBody.velocity = new Vector2(verticalVel, rigidBody.velocity.y);
//    //            break;
//    //    }
//    //}

//    //void VerticalMovement ()
//    //{
//    //    /
//    //    if (input.jumpPressed /*&& !isJumping*/ && (isOnGround || coyoteTime > Time.time))
//    //    {
//    //        //...The player is no longer on the groud and is jumping...
//    //        isOnGround = false;
            
//    //        //...add the jump force to the rigidbody...
//    //        rigidBody.AddForce(-GameManager.Instance.DirectionToGround * jumpForce, ForceMode2D.Impulse);
//    //    }
//    //    //If player is falling to fast, reduce the Y velocity to the max
//    //    if (DownwardsVelocity >= maxFallSpeed)
//    //        SetLocalVerticalVelocity(-maxFallSpeed);
//    //}

//    void FlipCharacterDirection ()
//    {
//        //Turn the character by flipping the direction
//        direction *= -1;

//        //Record the current scale
//        Vector3 scale = transform.localScale;

//        //Set the X scale to be the original times the direction
//        scale.x = originalXScale * direction;

//        //Apply the new scale
//        transform.localScale = scale;
//    }
    
//    ////These two Raycast methods wrap the Physics2D.Raycast() and provide some extra
//    ////functionality
//    //RaycastHit2D Raycast (Vector2 offset, Vector2 rayDirection, float length)
//    //{
//    //    //Call the overloaded Raycast() method using the ground layermask and return 
//    //    //the results
//    //    return Raycast(offset, rayDirection, length, groundLayer);
//    //}

//    //RaycastHit2D Raycast (Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
//    //{
//    //    //Record the player's position
//    //    Vector2 pos = transform.position;

//    //    //Send out the desired raycasr and record the result
//    //    RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

//    //    //If we want to show debug raycasts in the scene...
//    //    if (drawDebugRaycasts)
//    //    {
//    //        //...determine the color based on if the raycast hit...
//    //        Color color = hit ? Color.red : Color.green;
//    //        //...and draw the ray in the scene view
//    //        Debug.DrawRay(pos + offset, rayDirection * length, color);
//    //    }

//    //    //Return the results of the raycast
//    //    return hit;
//    //}

//    private void OnDestroy ()
//    {
//        GameManager.Instance.OnLevelStarted -= OnLevelStarted;
//        GameManager.Instance.OnLevelFailed -= OnLevelFailed;
//        GameManager.Instance.OnLevelGoalReached -= OnGoalreached;
//        GameManager.Instance.RemoveFixedUpdate(OnFixedUpdate);
//    }
//}

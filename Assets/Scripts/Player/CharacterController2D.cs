using System;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
    public LayerMask groundedLayerMask;
    [Tooltip("The distance down to check for ground.")]
    public float groundedRaycastDistance = 0.1f;

    public event Action OnLand;

    Rigidbody2D m_Rigidbody2D;
    Vector2 m_PreviousPosition;
    Vector2 m_CurrentPosition;
    Vector2 m_NextMovement;

    BoxCollider2D boxCollider;
    
    public float footOffset = .2f;          //X Offset of feet raycast

    public bool IsGrounded { get; protected set; }
    public bool IsCeilinged { get; protected set; }
    [SerializeField]
    public Vector2 Velocity { get; protected set; }

    public float DownwardsVelocity
    {
        get
        {
            switch (GameManager.Instance.GravityDirection)
            {
                case GravityDirection.UP:
                    return Velocity.y;
                case GravityDirection.DOWN:
                    return -Velocity.y;
                case GravityDirection.RIGHT:
                    return Velocity.x;
                case GravityDirection.LEFT:
                    return -Velocity.x;
                default:
                    return 0;
            }
        }
    }

    void Awake ()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        m_CurrentPosition = m_Rigidbody2D.position;
        m_PreviousPosition = m_Rigidbody2D.position;
    }

    void FixedUpdate ()
    {
        m_PreviousPosition = m_Rigidbody2D.position;
        m_CurrentPosition = m_PreviousPosition + m_NextMovement;
        Velocity = (m_CurrentPosition - m_PreviousPosition) / Time.fixedDeltaTime;

        m_Rigidbody2D.MovePosition(m_CurrentPosition);
        m_NextMovement = Vector2.zero;

        GroundCheck();
        CeilingCheck();

        //CheckCapsuleEndCollisions();
        //CheckCapsuleEndCollisions(false);
    }

    /// <summary>
    /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
    /// </summary>
    /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
    public void Move (Vector2 movement)
    {
        m_NextMovement += movement;
    }

    /// <summary>
    /// This moves the character without any implied velocity.
    /// </summary>
    /// <param name="position">The new position of the character in global space.</param>
    public void Teleport (Vector2 position)
    {
        Vector2 delta = position - m_CurrentPosition;
        m_PreviousPosition += delta;
        m_CurrentPosition = position;
        //m_Rigidbody2D.MovePosition(position);
        m_Rigidbody2D.position = position;
    }

    void GroundCheck ()
    {
        bool wasGrounded = IsGrounded;
        
        IsGrounded = CheckVerticalBoundsHits(GameManager.Instance.DirectionToGround);

        if (IsGrounded && !wasGrounded)
            OnLand?.Invoke();
    }

    void CeilingCheck ()
    {
        IsCeilinged = CheckVerticalBoundsHits(-GameManager.Instance.DirectionToGround, boxCollider.size.y);
    }

    bool CheckVerticalBoundsHits(Vector2 direction, float heightOffset = 0)
    {
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, heightOffset).Rotate(GameManager.Instance.WorldRotationAngle), direction, groundedRaycastDistance, groundedLayerMask);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, heightOffset).Rotate(GameManager.Instance.WorldRotationAngle), direction, groundedRaycastDistance, groundedLayerMask);

        return leftCheck || rightCheck;
    }

    RaycastHit2D Raycast (Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        //Record the player's position
        Vector2 pos = transform.position;

        //Send out the desired raycasr and record the result
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        //If we want to show debug raycasts in the scene...
        bool drawDebugRaycasts = true;
        if (drawDebugRaycasts)
        {
            //...determine the color based on if the raycast hit...
            Color color = hit ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay(pos + offset, rayDirection * length, color);
        }

        //Return the results of the raycast
        return hit;
    }
}
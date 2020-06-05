using System.Linq;
using UnityEngine;
using UnityEngine.Lumin;

[RequireComponent(typeof(PlatformCatcher))]
public class IceFloor : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D playerRigidbody2D;
    float horizontalVelocity;

    PlatformCatcher platformCatcher;

    void Awake()
    {
        platformCatcher = GetComponent<PlatformCatcher>();

        platformCatcher.OnCaught += OnCaught;
        platformCatcher.OnLost += OnLost;
    }


    void OnCaught (PlatformCatcher.CaughtObject caughtObj)
    {
        if (caughtObj.collider.CompareTag("Player"))
        {
            if (player == null)
                player = caughtObj.collider.GetComponent<PlayerController>();

            player.isSliding = true;
        }
    }

    void OnLost(PlatformCatcher.CaughtObject lostObj)
    {
        if (lostObj.collider.CompareTag("Player"))
        {
            player.isSliding = false;
        }
    }

    void OnDestroy()
    {
        platformCatcher.OnCaught -= OnCaught;
        platformCatcher.OnLost -= OnLost;
    }

    //void OnCollisionEnter2D (Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        player = collision.gameObject.GetComponent<PlayerController>();
    //        player.isSliding = true;
    //    }
    //}

    //void OnCollisionExit2D (Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        player.isSliding = false;
    //        player = null;
    //    }
    //}
}
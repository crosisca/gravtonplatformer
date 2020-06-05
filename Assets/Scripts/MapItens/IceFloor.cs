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
            Debug.Log("Caught Player");

        }
    }

    void OnLost(PlatformCatcher.CaughtObject lostObj)
    {
        if (lostObj.collider.CompareTag("Player"))
        {
            player.isSliding = false;
            Debug.Log("Lost Player");
        }
    }

    void OnDestroy()
    {
        platformCatcher.OnCaught -= OnCaught;
        platformCatcher.OnLost -= OnLost;
    }
}
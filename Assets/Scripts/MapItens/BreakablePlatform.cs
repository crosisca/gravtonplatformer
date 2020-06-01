using System.Collections.Generic;
using MEC;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class BreakablePlatform : MapItem
{
    protected BoxCollider2D boxCollider2d;

    [SerializeField]
    float breakDelay = 2;

    CoroutineHandle breakCoroutine;

    Renderer[] renderers;

    protected override void Awake()
    {
        base.Awake();
        
        boxCollider2d = GetComponent<BoxCollider2D>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public override void Activate()
    {
        base.Activate();

        Timing.KillCoroutines(breakCoroutine);
        boxCollider2d.enabled = true;

        SetOpacity(1);
    }

    void SetOpacity(float alpha)
    {
        foreach (Renderer renderer in renderers)
        {
            Color newColor = renderer.material.color;
            newColor.a = alpha;
            renderer.material.color = newColor;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);

        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Platform will break in {breakDelay} seconds");
            Timing.KillCoroutines(breakCoroutine);
            breakCoroutine = Timing.RunCoroutine(BreakCoroutine());
        }
    }

    IEnumerator<float> BreakCoroutine()
    {
        yield return Timing.WaitForSeconds(breakDelay);
        Deactivate();
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        SetOpacity(0.5f);
        boxCollider2d.enabled = false;
    }

    protected override void OnDestroy()
    {
        Timing.KillCoroutines(breakCoroutine);

        base.OnDestroy();
    }
}
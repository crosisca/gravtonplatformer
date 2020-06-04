using UnityEngine;

public class MapItemActivator : MapItem
{
    [SerializeField]
    MapItem[] connectedItens = new MapItem[0];

    protected override void OnTriggerEnter2D (Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            foreach (MapItem connectedIten in connectedItens)
                connectedIten.Activate();
        }
    }
}
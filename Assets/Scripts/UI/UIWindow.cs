﻿using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public bool isOpen { get; private set; } = false;

    public virtual void Open()
    {
        isOpen = true;
        gameObject.SetActive(true);
        OnOpen();
    }

    public void Close()
    {
        isOpen = false;
        OnClose();
        gameObject.SetActive(false);
    }

    protected abstract void OnOpen();
    protected abstract void OnClose();
}
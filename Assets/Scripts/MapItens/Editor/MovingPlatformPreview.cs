﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MovingPlatformPreview 
{
    public static MovingPlatformPreview s_Preview = null;
    public static GameObject preview;

    protected static MovingPlatform movingPlatform;

    static MovingPlatformPreview()
    {
        Selection.selectionChanged += SelectionChanged;
    }

    static void SelectionChanged()
    {
        if (movingPlatform != null && Selection.activeGameObject != movingPlatform.gameObject)
        {
            DestroyPreview();
        }
    }

    public static void DestroyPreview()
    {
        if (preview == null)
            return;

        Object.DestroyImmediate(preview);
        preview = null;
        movingPlatform = null;
    }

    public static void CreateNewPreview(MovingPlatform origin)
    {
        if(preview != null)
        {
            Object.DestroyImmediate(preview);
        }

        movingPlatform = origin; 

        preview = Object.Instantiate(origin.gameObject);
        preview.hideFlags = HideFlags.DontSave;
        MovingPlatform plt = preview.GetComponentInChildren<MovingPlatform>();
        Object.DestroyImmediate(plt);


        Color c = new Color(0.2f, 0.2f, 0.2f, 0.4f);
        SpriteRenderer[] rends = preview.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < rends.Length; ++i)
            rends[i].color = c;
    }
}
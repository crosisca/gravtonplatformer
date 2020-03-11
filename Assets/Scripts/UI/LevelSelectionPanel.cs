using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionPanel : UIWindow
{

    public event Action<int, int> OnLevelSelected;

    private void Start ()
    {
        Button[] lvlButtons = transform.FindDeepChild("World1Levels").transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < lvlButtons.Length; i++)
        {
            int n = i + 1;
            lvlButtons[i].onClick.AddListener(() => OnLevelButtonClicked(n));
        }
    }


    public void OnLevelButtonClicked (int lvlNumber)
    {
        OnLevelSelected?.Invoke(1, lvlNumber);
    }
}

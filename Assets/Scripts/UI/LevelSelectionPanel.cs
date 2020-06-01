using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionPanel : UIWindow
{

    public event Action<int, int> OnLevelSelected;

    int selectedWorldNumber = 1;

    private void Start ()
    {
        Button[] worldButtons = transform.FindDeepChild("Worlds").transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < worldButtons.Length; i++)
        {
            int n = i + 1;
            worldButtons[i].onClick.AddListener(() => OnWorldButtonClicked(n));
        }

        Button[] lvlButtons = transform.FindDeepChild("Levels").transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < lvlButtons.Length; i++)
        {
            int n = i + 1;
            lvlButtons[i].onClick.AddListener(() => OnLevelButtonClicked(n));
        }
    }

    void OnWorldButtonClicked(int worldNumber)
    {
        selectedWorldNumber = worldNumber;
    }


    public void OnLevelButtonClicked (int lvlNumber)
    {
        OnLevelSelected?.Invoke(selectedWorldNumber, lvlNumber);
    }
}

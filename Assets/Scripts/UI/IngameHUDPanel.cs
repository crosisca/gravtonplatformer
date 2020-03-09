using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IngameHUDPanel : UIWindow
{
    Button pauseButton;
    Button restartButton;

    private void Awake ()
    {
        pauseButton = transform.FindDeepChild("Button_Pause").GetComponent<Button>();
        restartButton = transform.FindDeepChild("Button_Restart").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start ()
    {
        pauseButton.onClick.AddListener(OnClickPause);
        restartButton.onClick.AddListener(OnClickRestart);
    }

    private void OnClickPause ()
    {
        GameManager.Instance.TogglePause(true);
    }

    private void OnClickRestart ()
    {
        GameManager.Instance.RestartLevel();
    }

    protected override void OnOpen ()
    {
    }

    protected override void OnClose ()
    {
    }
}

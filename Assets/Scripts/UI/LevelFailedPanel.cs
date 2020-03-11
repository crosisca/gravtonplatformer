using System;
using System.Collections;
using UnityEngine.UI;

public class LevelFailedPanel : UIWindow
{
    Button homeButton;
    Button restartButton;

    private void Awake ()
    {
        homeButton = transform.FindDeepChild("Button_Home").GetComponent<Button>();
        restartButton = transform.FindDeepChild("Button_Restart").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        homeButton.onClick.AddListener(OnClickHome);
        restartButton.onClick.AddListener(OnClickRestart);

        Close();
    }

    private void OnClickRestart ()
    {
        GameManager.Instance.RestartLevel();
        Close();
    }
    private void OnClickHome ()
    {
        GameManager.Instance.FinishLevel();
        Close();
    }

    protected override void OnOpen ()
    {
    }

    protected override void OnClose ()
    {
    }
}

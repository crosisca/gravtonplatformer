using System;
using System.Collections;
using UnityEngine.UI;

public class PausePanel : UIWindow
{
    Button resumeButton;
    Button homeButton;
    Button restartButton;

    private void Awake ()
    {
        resumeButton = transform.FindDeepChild("Button_Resume").GetComponent<Button>();
        homeButton = transform.FindDeepChild("Button_Home").GetComponent<Button>();
        restartButton = transform.FindDeepChild("Button_Restart").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        resumeButton.onClick.AddListener(OnClickResume);
        homeButton.onClick.AddListener(OnClickHome);
        restartButton.onClick.AddListener(OnClickRestart);

        ApplicationManager.Instance.OnGameSessionStarted += OnGameSessionStarted;
        ApplicationManager.Instance.OnGameSessionFinished += OnGameSessionFinished;

        Close();
    }

    private void OnGameSessionStarted ()
    {
        GameManager.Instance.OnPauseChanged += OnPauseChanged;
    }

    private void OnGameSessionFinished ()
    {
        GameManager.Instance.OnPauseChanged -= OnPauseChanged;
    }

    private void OnPauseChanged (bool isPaused)
    {
        if (isPaused)
            Open();
        else
            Close();
    }
    
    private void OnClickResume ()
    {
        GameManager.Instance.TogglePause(false);
    }

    private void OnClickRestart ()
    {
        GameManager.Instance.RestartLevel();
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

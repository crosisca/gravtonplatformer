// This script handles inputs for the player. It serves two main purposes: 1) wrap up
// inputs so swapping between mobile and standalone is simpler and 2) keeping inputs
// from Update() in sync with FixedUpdate()
using System;
using UnityEngine;

//We first ensure this script runs before all other player scripts to prevent laggy
//inputs
[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{
	public float horizontal;      //Float that stores horizontal input
	public bool jumpHeld;         //Bool that stores jump pressed
	public bool jumpPressed;      //Bool that stores jump held
	public bool crouchHeld;       //Bool that stores crouch pressed
	public bool crouchPressed;    //Bool that stores crouch held

	bool readyToClear;                              //Bool used to keep input in sync


	PlayerInputActions inputActions;

    PlayerController player;

	private void Awake()
	{
		inputActions = new PlayerInputActions();
	    player = GetComponent<PlayerController>();

        GameManager.Instance.OnLevelStarted += OnLevelStarted;
	    GameManager.Instance.OnLevelFailed += OnLevelFailed;
        GameManager.Instance.OnLevelFinished += OnLevelFinished;
	    player.OnDeath += OnPlayerDeath;
	}

    void OnLevelStarted ()
	{
		inputActions.Enable();
	}

    void OnLevelFailed (int arg1, int arg2)
    {
        inputActions.Disable();
    }

    void OnLevelFinished (int worldNumber, int levelNumber)
	{
		inputActions.Disable();

        GameManager.Instance.OnLevelStarted -= OnLevelStarted;
	    GameManager.Instance.OnLevelFailed -= OnLevelFailed;
        GameManager.Instance.OnLevelFinished -= OnLevelFinished;
	    player.OnDeath -= OnPlayerDeath;
	}

    void OnPlayerDeath ()
    {
        inputActions.Disable();
    }

    void Update()
	{
		//Clear out existing input values
		ClearInput();

		//If the Game Manager says the game is over, exit
		//if (GameManager.IsGameOver())
		//	return;

		ProcessInputs();

		//Clamp the horizontal input to be between -1 and 1
		horizontal = Mathf.Clamp(horizontal, -1f, 1f);
	}

	void FixedUpdate()
	{
		//In FixedUpdate() we set a flag that lets inputs to be cleared out during the 
		//next Update(). This ensures that all code gets to use the current inputs
		readyToClear = true;
	}

	void ClearInput()
	{
		//If we're not ready to clear input, exit
		if (!readyToClear)
			return;

		//Reset all inputs
		horizontal = 0f;
		jumpPressed = false;
		jumpHeld = false;
		crouchPressed = false;
		crouchHeld = false;

		readyToClear = false;
	}

	void ProcessInputs()
	{
		horizontal = inputActions.PlayerActions.Move.ReadValue<float>();

		jumpPressed = jumpPressed || inputActions.PlayerActions.Jump.triggered;

		jumpHeld = jumpHeld || (!inputActions.PlayerActions.Jump.triggered && (inputActions.PlayerActions.Jump.ReadValue<float>() == 1));
	}
}
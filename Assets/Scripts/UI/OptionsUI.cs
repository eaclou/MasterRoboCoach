using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {

    public GameManager gameManager;
    //public Button buttonBack;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnterState() {
        InitializeUIFromGameState();
    }

    public void ExitState() {

    }

    public void InitializeUIFromGameState() {
        
    }

    public void ClickBack() {
        gameManager.ChangeState(GameManager.GameState.MainMenu);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentUI : MonoBehaviour {

    public MainMenu mainMenuRef;
    public GameObject panelTournamentOverview;
    public GameObject panelTournamentActive;

    public Button buttonPlayMatch;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(TournamentInfo tournamentInfo) {
        panelTournamentOverview.SetActive(true);
        panelTournamentActive.SetActive(false);
    }

    public void ClickPlayMatch() {
        panelTournamentOverview.SetActive(false);
        panelTournamentActive.SetActive(true);

        // INITIALIZE TOURNAMENT MANAGER
        mainMenuRef.gameManagerRef.tournamentManager.PlayNextMatch();

    }
}

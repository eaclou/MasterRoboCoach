using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTrajectoryUI : MonoBehaviour {

    public TrajectorySensorGenome genome;

    public Text textModuleDescription;

    //public Slider sliderValue;
    //public Text textValueValue;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Trajectory Sensor " + genome.inno.ToString();
        //sliderValue.value = genome.val;
        //textValueValue.text = genome.val.ToString();
    }

    /*public void SliderValue(float value) {
        genome.val = value;
        textValueValue.text = genome.val.ToString();
    }  */  
}

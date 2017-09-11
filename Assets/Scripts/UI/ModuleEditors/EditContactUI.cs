using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditContactUI : MonoBehaviour {

    public ContactGenome genome;
    
    public Text textModuleDescription;

    public Slider sliderParentID;
    public Text textParentIDValue;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Contact Sensor " + genome.inno.ToString();
        sliderParentID.minValue = 0;
        sliderParentID.maxValue = 8;
        sliderParentID.value = genome.parentID;
        textParentIDValue.text = genome.parentID.ToString();
    }

    public void SliderParentID(float value) {

        genome.parentID = Mathf.RoundToInt(value);
        textParentIDValue.text = genome.parentID.ToString();
    }    
}

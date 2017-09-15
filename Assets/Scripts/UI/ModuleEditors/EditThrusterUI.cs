using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditThrusterUI : MonoBehaviour {

    public ThrusterGenome genome;

    public Text textModuleDescription;

    public Slider sliderPositionX;
    public Text textPositionXValue;
    public Slider sliderPositionY;
    public Text textPositionYValue;
    public Slider sliderPositionZ;
    public Text textPositionZValue;

    public Slider sliderHorsepowerX;
    public Text textHorsepowerXValue;
    public Slider sliderHorsepowerY;
    public Text textHorsepowerYValue;
    public Slider sliderHorsepowerZ;
    public Text textHorsepowerZValue;

    public Toggle toggleUseX;
    public Toggle toggleUseY;
    public Toggle toggleUseZ;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Thruster " + genome.inno.ToString();
        sliderPositionX.value = genome.forcePoint.x;
        textPositionXValue.text = genome.forcePoint.x.ToString();
        sliderPositionY.value = genome.forcePoint.y;
        textPositionYValue.text = genome.forcePoint.y.ToString();
        sliderPositionZ.value = genome.forcePoint.z;
        textPositionZValue.text = genome.forcePoint.z.ToString();

        sliderHorsepowerX.value = genome.horsepowerX;
        textHorsepowerXValue.text = genome.horsepowerX.ToString();
        sliderHorsepowerY.value = genome.horsepowerY;
        textHorsepowerYValue.text = genome.horsepowerY.ToString();
        sliderHorsepowerZ.value = genome.horsepowerZ;
        textHorsepowerZValue.text = genome.horsepowerZ.ToString();

        toggleUseX.isOn = genome.useX;
        toggleUseY.isOn = genome.useY;
        toggleUseZ.isOn = genome.useZ;
    }

    public void SliderPositionX(float value) {
        genome.forcePoint.x = value;
        textPositionXValue.text = genome.forcePoint.x.ToString();
    }
    public void SliderPositionY(float value) {
        genome.forcePoint.y = value;
        textPositionYValue.text = genome.forcePoint.y.ToString();
    }
    public void SliderPositionZ(float value) {
        genome.forcePoint.z = value;
        textPositionZValue.text = genome.forcePoint.z.ToString();
    }
    public void SliderHorsepowerX(float value) {
        genome.horsepowerX = value;
        textHorsepowerXValue.text = genome.horsepowerX.ToString();
    }
    public void SliderHorsepowerY(float value) {
        genome.horsepowerY = value;
        textHorsepowerYValue.text = genome.horsepowerY.ToString();
    }
    public void SliderHorsepowerZ(float value) {
        genome.horsepowerZ = value;
        textHorsepowerZValue.text = genome.horsepowerZ.ToString();
    }
    public void ToggleUseX(bool value) {
        genome.useX = value;
    }
    public void ToggleUseY(bool value) {
        genome.useY = value;
    }
    public void ToggleUseZ(bool value) {
        genome.useZ = value;
    }
}

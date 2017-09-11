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
    public Slider sliderHorsepowerZ;
    public Text textHorsepowerZValue;


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
        sliderHorsepowerZ.value = genome.horsepowerZ;
        textHorsepowerZValue.text = genome.horsepowerZ.ToString();
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

    public void SliderHorsepowerZ(float value) {

        genome.horsepowerZ = value;
        textHorsepowerZValue.text = genome.horsepowerZ.ToString();
    }
}

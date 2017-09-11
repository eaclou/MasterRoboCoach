using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTargetUI : MonoBehaviour {

    public TargetSensorGenome genome;

    public Text textModuleDescription;

    public Slider sliderPositionX;
    public Text textPositionXValue;
    public Slider sliderPositionY;
    public Text textPositionYValue;
    public Slider sliderPositionZ;
    public Text textPositionZValue;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Target Sensor " + genome.inno.ToString();
        sliderPositionX.value = genome.sensorPosition.x;
        textPositionXValue.text = genome.sensorPosition.x.ToString();
        sliderPositionY.value = genome.sensorPosition.y;
        textPositionYValue.text = genome.sensorPosition.y.ToString();
        sliderPositionZ.value = genome.sensorPosition.z;
        textPositionZValue.text = genome.sensorPosition.z.ToString();
    }

    public void SliderPositionX(float value) {

        genome.sensorPosition.x = value;
        textPositionXValue.text = genome.sensorPosition.x.ToString();
    }

    public void SliderPositionY(float value) {

        genome.sensorPosition.y = value;
        textPositionYValue.text = genome.sensorPosition.y.ToString();
    }

    public void SliderPositionZ(float value) {

        genome.sensorPosition.z = value;
        textPositionZValue.text = genome.sensorPosition.z.ToString();
    }
}

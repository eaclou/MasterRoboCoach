using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditObstaclesUI : MonoBehaviour {

    public BasicObstaclesGenome genome;

    public Text textModuleDescription;

    //public Toggle toggleAltitude;
    //public Text textFrequencyValue;
    public Slider sliderNumObstacles;
    public Text textNumObstaclesValue;
    public Slider sliderMinSize;
    public Text textMinSizeValue;
    public Slider sliderMaxSize;
    public Text textMaxSizeValue;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Basic Obstacles (Boulders)";
        //toggleAltitude.isOn = genome.useAltitude;
        //textFrequencyValue.text = genome.freq.ToString();
        sliderNumObstacles.value = genome.numObstacles;
        textNumObstaclesValue.text = genome.numObstacles.ToString();
        sliderMinSize.value = genome.minObstacleSize;
        textMinSizeValue.text = genome.minObstacleSize.ToString();
        sliderMaxSize.value = genome.maxObstacleSize;
        textMaxSizeValue.text = genome.maxObstacleSize.ToString();
    }
    
    public void SliderNumObstacles(float value) {
        genome.numObstacles = Mathf.RoundToInt(value);
        textNumObstaclesValue.text = genome.numObstacles.ToString();
    }
    public void SliderMinSize(float value) {
        genome.minObstacleSize = value;
        textMinSizeValue.text = genome.minObstacleSize.ToString();
    }
    public void SliderMaxSize(float value) {
        genome.maxObstacleSize = value;
        textMaxSizeValue.text = genome.maxObstacleSize.ToString();
    }
}

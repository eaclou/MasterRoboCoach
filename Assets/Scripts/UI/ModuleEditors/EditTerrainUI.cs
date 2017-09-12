using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTerrainUI : MonoBehaviour {

    public TerrainGenome genome;

    public Text textModuleDescription;

    public Toggle toggleAltitude;
    //public Text textFrequencyValue;
    public Slider sliderNumOctaves;
    public Text textNumOctavesValue;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Terrain " + genome.color.ToString();
        toggleAltitude.isOn = genome.useAltitude;
        //textFrequencyValue.text = genome.freq.ToString();
        sliderNumOctaves.value = genome.numOctaves;
        textNumOctavesValue.text = genome.numOctaves.ToString();
    }

    public void ToggleUseAltitude(bool value) {
        genome.useAltitude = value;
        //textFrequencyValue.text = genome.freq.ToString();
    }

    public void SliderNumOctaves(float value) {
        genome.numOctaves = Mathf.RoundToInt(value);
        textNumOctavesValue.text = genome.numOctaves.ToString();
    }
}

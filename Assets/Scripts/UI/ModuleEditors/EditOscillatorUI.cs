using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditOscillatorUI : MonoBehaviour {

    public OscillatorGenome genome;

    public Text textModuleDescription;

    public Slider sliderFrequency;
    public Text textFrequencyValue;
    public Slider sliderAmplitude;
    public Text textAmplitudeValue;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Oscillator " + genome.inno.ToString();
        sliderFrequency.value = genome.freq;
        textFrequencyValue.text = genome.freq.ToString();
        sliderAmplitude.value = genome.amp;
        textAmplitudeValue.text = genome.amp.ToString();
    }

    public void SliderFrequency(float value) {

        genome.freq = value;
        textFrequencyValue.text = genome.freq.ToString();
    }

    public void SliderAmplitude(float value) {

        genome.amp = value;
        textAmplitudeValue.text = genome.amp.ToString();
    }
}

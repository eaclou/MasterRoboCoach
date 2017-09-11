using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditWeaponTazerUI : MonoBehaviour {

    public WeaponTazerGenome genome;

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
        textModuleDescription.text = "Weapon (Tazer) " + genome.inno.ToString();
        sliderPositionX.value = genome.muzzleLocation.x;
        textPositionXValue.text = genome.muzzleLocation.x.ToString();
        sliderPositionY.value = genome.muzzleLocation.y;
        textPositionYValue.text = genome.muzzleLocation.y.ToString();
        sliderPositionZ.value = genome.muzzleLocation.z;
        textPositionZValue.text = genome.muzzleLocation.z.ToString();
    }

    public void SliderPositionX(float value) {
        genome.muzzleLocation.x = value;
        textPositionXValue.text = genome.muzzleLocation.x.ToString();
    }
    public void SliderPositionY(float value) {
        genome.muzzleLocation.y = value;
        textPositionYValue.text = genome.muzzleLocation.y.ToString();
    }
    public void SliderPositionZ(float value) {
        genome.muzzleLocation.z = value;
        textPositionZValue.text = genome.muzzleLocation.z.ToString();
    }
}

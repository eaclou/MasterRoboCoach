using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodadManager : MonoBehaviour {

    public Material lightMaterial_01;
    public Material lightMaterial_02;
    public Transform doodadXForm_01;

    public int neuronID_01 = 0;
    public int neuronID_02 = 0;
    public int neuronID_03 = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Tick(float neuronVal01, float neuronVal02, float neuronVal03) {
        if(lightMaterial_01 != null) {
            lightMaterial_01.SetFloat("_LightValue", neuronVal01);
        }
        if (lightMaterial_02 != null) {
            lightMaterial_02.SetFloat("_LightValue", neuronVal02);
        }
    }
}

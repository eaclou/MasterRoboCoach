using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodadManager : MonoBehaviour {

    public Material[] lightMaterialArray;
   // public Material lightMaterial_01;
    //public Material lightMaterial_02;
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

        if(lightMaterialArray != null) {
            for(int i = 0; i < lightMaterialArray.Length; i++) {
                float neuronValue = 0f;
                if(i % 3 == 0) {
                    neuronValue = neuronVal01;
                }
                if (i % 3 == 1) {
                    neuronValue = neuronVal02;
                }
                if (i % 3 == 2) {
                    neuronValue = neuronVal03;
                }
                lightMaterialArray[i].SetFloat("_LightValue", neuronValue);
            }
        }
    }
}

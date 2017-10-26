using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotateObjectYAxis : MonoBehaviour {

    public float rotationRate = 0.5f;
    public float rotateZ = 150f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(0f, Time.fixedTime * rotationRate, rotateZ);
	}
}

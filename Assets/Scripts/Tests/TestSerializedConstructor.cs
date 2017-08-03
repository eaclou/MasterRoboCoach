using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSerializedConstructor : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        Instantiate(prefab);
	}
	
	// Update is called once per frame
	void Update () {
        //gameObject.transform.forward;
	}
}

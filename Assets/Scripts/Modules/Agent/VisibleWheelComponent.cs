using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleWheelComponent : MonoBehaviour {

    public WheelCollider wheelCollider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);

        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation * Quaternion.Euler(new Vector3(0f, 0f, 90f));
    }
}

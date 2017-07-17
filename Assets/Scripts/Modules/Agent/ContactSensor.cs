using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactSensor : MonoBehaviour {
    public bool contact = false;

    private void FixedUpdate() {
        contact = false;
    }

    private void OnCollisionEnter(Collision collision) {
               
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.collider.tag == "hazard") {
            contact = true;
        }
    }
}

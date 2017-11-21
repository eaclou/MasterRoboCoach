using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactSensorComponent : MonoBehaviour {
    
    public bool contact = false;
    public bool hazard = false;
    public float maxImpactForce = 0f;
    public bool newCollision = false;

    public ContactSensor sensor;

    void Awake() {
        
    }

    private void FixedUpdate() {
        contact = false;
        hazard = false;
        maxImpactForce = 0f;
        newCollision = false;
    }

    private void OnCollisionEnter(Collision collision) {
        //processCollisionForces(collision.impulse.magnitude);
        processCollisionForces(collision.relativeVelocity.magnitude);
        //Debug.Log("OnCollisionEnter: " + collision.impulse.magnitude.ToString());
        newCollision = true;

        if (collision.collider.tag == "hazard") {
            hazard = true;
        }
        contact = true;
    }

    private void OnCollisionStay(Collision collision) {
        processCollisionForces(collision.impulse.magnitude);
        if (collision.collider.tag == "hazard") {
            hazard = true;
        }
        contact = true;
    }

    private void processCollisionForces(float collisionForce) {
        maxImpactForce = Mathf.Max(maxImpactForce, collisionForce);
        //maxImpactForce = collisionForce;
    }
}

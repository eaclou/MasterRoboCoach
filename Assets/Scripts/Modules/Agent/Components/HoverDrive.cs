using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverDrive : MonoBehaviour {

    public float hoverHeightVel = 0f;
    public float hoverDesiredHeight = 0.12f;
    public float hoverDampen = 0.9f;
    public float hoverThrust = 1f;

    public float horizontalDrag = 0.8f;

    public float debugDist;
    public float debugThrust;

    void Start() {
        //hoverHeight = hoverDesiredHeight;
        
    }

    void FixedUpdate () {
        Vector3 rayOrigin = gameObject.transform.position;
        Vector3 rayDirection = new Vector3(0f, -1f, 0f);

        RaycastHit hit;

        float rayMaxDistance = 2f;
        float distance = rayMaxDistance;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayMaxDistance)) {
            distance = hit.distance;
        }

        //float deltaHover = 1f;        
        
        float deltaHover = (distance - hoverDesiredHeight) * -1f;
        float thrust = 0f; // = deltaHover * hoverThrust;
        if(deltaHover > 0) {
            thrust = 1f * hoverThrust;
        }
        //float thrust = deltaHover * hoverThrust;
        if (distance >= rayMaxDistance * 0.9f) {
            thrust = 0f;
        }

        Vector3 vel = gameObject.GetComponent<Rigidbody>().velocity;
        vel.y *= hoverDampen;

        vel.x *= horizontalDrag;
        vel.z *= horizontalDrag;
        gameObject.GetComponent<Rigidbody>().velocity = vel;
        gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0f, thrust * deltaHover, 0f), ForceMode.Force);

        debugDist = distance;
        debugThrust = (thrust * deltaHover);
        //Debug.Log("dist: " + distance.ToString() + ", thrust: " + (thrust * deltaHover).ToString());

        // Rotation!!!
        Quaternion currentRotation = gameObject.transform.rotation;
        //Quaternion targetRotation = Vector3.Cross(gameObject.transform.forward, new Vector3(0f, 0f, 1f));
        //Quaternion.LookRotation(gameObject.transform.forward, new Vector3(0f, 0f, 1f));
        //gameObject.GetComponent<Rigidbody>().ace
        Vector3 headingX = new Vector3(gameObject.transform.right.x, 0f, gameObject.transform.right.z);
        Vector3 headingZ = new Vector3(gameObject.transform.forward.x, 0f, gameObject.transform.forward.z);
        
        Quaternion targetLevelRotation = Quaternion.LookRotation(headingZ, new Vector3(0f, 1f, 0f));

        Quaternion targetRot = Quaternion.FromToRotation(gameObject.transform.forward, headingZ);
        Vector3 x = Vector3.Cross(gameObject.transform.forward.normalized, headingZ.normalized);
        float theta = Mathf.Asin(x.magnitude);
        Vector3 w = x.normalized * theta;
        Quaternion q = transform.rotation * gameObject.GetComponent<Rigidbody>().inertiaTensorRotation;
        Vector3 t = q * Vector3.Scale(gameObject.GetComponent<Rigidbody>().inertiaTensor, (Quaternion.Inverse(q) * w));
        //gameObject.GetComponent<Rigidbody>().AddTorque(t * 10f, ForceMode.Impulse);
        //gameObject.GetComponent<Rigidbody>().AddTorque();

        float dotZ = Vector3.Dot(headingZ.normalized, gameObject.transform.forward);
        float dotY = Vector3.Dot(new Vector3(0f, 1f, 0f), gameObject.transform.up);
        float dotX = Vector3.Dot(headingX.normalized, gameObject.transform.right);
        //if()
        float xTorque = Vector3.Dot(headingX.normalized, gameObject.transform.up) * 2f;
        float zTorque = -Vector3.Dot(headingZ.normalized, gameObject.transform.up) * 2f;

        gameObject.GetComponent<Rigidbody>().AddRelativeTorque(0f, 0f, xTorque);
        gameObject.GetComponent<Rigidbody>().AddRelativeTorque(zTorque, 0f, 0f);
        
        
        //gameObject.GetComponent<Rigidbody>().AddTorque(-Vector3.Cross(headingZ, gameObject.transform.forward) * delta);
        //Debug.Log(xTorque.ToString());

        //
    }
}

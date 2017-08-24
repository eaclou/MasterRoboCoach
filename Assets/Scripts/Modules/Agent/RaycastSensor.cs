using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RaycastSensor : AgentModuleBase {

    //public int parentID;
    //public int inno;    
    public float[] distanceLeft;
    public float[] distanceLeftCenter;
    public float[] distanceCenter;
    public float[] distanceRightCenter;
    public float[] distanceRight;
    public float[] distanceBack;
    public float[] distanceCenterShort;

    public GameObject parentObject;
    public Vector3 sensorPosition;

    public Transform targetPosition;    

    public RaycastSensor() {
        /*
        parentID = genome.parentID;
        inno = genome.inno;
        distanceLeft = new float[1];
        distanceLeftCenter = new float[1];
        distanceCenter = new float[1];
        distanceRightCenter = new float[1];
        distanceRight = new float[1];
        distanceBack = new float[1];
        distanceCenterShort = new float[1];
        */
    }

    public void Initialize(RaycastSensorGenome genome, Agent agent) {
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;

        distanceLeft = new float[1];
        distanceLeftCenter = new float[1];
        distanceCenter = new float[1];
        distanceRightCenter = new float[1];
        distanceRight = new float[1];
        distanceBack = new float[1];
        distanceCenterShort = new float[1];

        parentObject = agent.segmentList[parentID];
        sensorPosition = genome.sensorPosition;
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = distanceLeft;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = distanceLeftCenter;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 2) {
                neuron.currentValue = distanceCenter;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 3) {
                neuron.currentValue = distanceRightCenter;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 4) {
                neuron.currentValue = distanceRight;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 5) {
                neuron.currentValue = distanceBack;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 6) {
                neuron.currentValue = distanceCenterShort;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
    }

    public void Tick() {
        Vector3 rayOrigin = parentObject.transform.position + sensorPosition;
        Vector3 left = parentObject.transform.TransformDirection(new Vector3(-1f, 0f, 0f).normalized);
        Vector3 leftCenter = parentObject.transform.TransformDirection(new Vector3(-1f, 0f, 1f).normalized);
        Vector3 center = parentObject.transform.TransformDirection(new Vector3(0f, 0f, 1f));
        Vector3 rightCenter = parentObject.transform.TransformDirection(new Vector3(1f, 0f, 1f).normalized);
        Vector3 right = parentObject.transform.TransformDirection(new Vector3(1f, 0f, 0f).normalized);
        Vector3 back = parentObject.transform.TransformDirection(new Vector3(0f, 0f, -1f));

        RaycastHit hit;

        float rayMaxDistance = 25f;
        float sensitivity = 1f / rayMaxDistance;
        //Debug.Log("raycastSensorList: ");
        distanceLeft[0] = rayMaxDistance * sensitivity;
        distanceLeftCenter[0] = rayMaxDistance * sensitivity;
        distanceCenter[0] = rayMaxDistance * sensitivity;
        distanceRightCenter[0] = rayMaxDistance * sensitivity;
        distanceRight[0] = rayMaxDistance * sensitivity;
        distanceBack[0] = rayMaxDistance * sensitivity;
        distanceCenterShort[0] = rayMaxDistance * sensitivity;

        if (Physics.Raycast(rayOrigin, left, out hit, rayMaxDistance)) {
            if (hit.collider.tag == "hazard")
                distanceLeft[0] = hit.distance * sensitivity;
        }
        if (Physics.Raycast(rayOrigin, leftCenter, out hit, rayMaxDistance)) {
            if (hit.collider.tag == "hazard")
                distanceLeftCenter[0] = hit.distance * sensitivity;
        }
        if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance * 3f)) {
            if (hit.collider.tag == "hazard")
                distanceCenter[0] = hit.distance * sensitivity / 3f;
        }
        if (Physics.Raycast(rayOrigin, rightCenter, out hit, rayMaxDistance)) {
            if (hit.collider.tag == "hazard")
                distanceRightCenter[0] = hit.distance * sensitivity;
        }
        if (Physics.Raycast(rayOrigin, right, out hit, rayMaxDistance)) {
            if (hit.collider.tag == "hazard")
                distanceRight[0] = hit.distance * sensitivity;
        }
        if (Physics.Raycast(rayOrigin, back, out hit, rayMaxDistance)) {
            if (hit.collider.tag == "hazard")
                distanceBack[0] = hit.distance * sensitivity;
        }
        if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance)) {
            if (hit.collider.tag == "hazard")
                distanceCenterShort[0] = hit.distance * sensitivity;
        }

        distanceLeft[0] = 1f - distanceLeft[0];
        distanceLeftCenter[0] = 1f - distanceLeftCenter[0];
        distanceCenter[0] = 1f - distanceCenter[0];
        distanceRightCenter[0] = 1f - distanceRightCenter[0];
        distanceRight[0] = 1f - distanceRight[0];
        distanceBack[0] = 1f - distanceBack[0];
        distanceCenterShort[0] = 1f - distanceCenterShort[0];
    }
}

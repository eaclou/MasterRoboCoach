using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicJoint {

    public int parentID;
    public int inno;
    public ConfigurableJoint joint;
    private Quaternion restRotation;
    public float motorStrength;
    public float angleSensitivity;

    public float[] throttleX;
    public float[] throttleY;
    public float[] throttleZ;

    public float[] angleX;
    public float[] angleY;
    public float[] angleZ;

    public bool useX;
    public bool useY;
    public bool useZ;

    public BasicJoint() {
        //parentID = genome.parentID;
        //inno = genome.inno;
        //throttle = new float[1];
        //strafe = new float[1];
        //throttle[0] = 0f;
    }

    public void Initialize(BasicJointGenome genome) {
        //Debug.Log("Init Joint! " + inno.ToString());
        inno = genome.inno;
        throttleX = new float[1];
        throttleY = new float[1];
        throttleZ = new float[1];
        angleX = new float[1];
        angleY = new float[1];
        angleZ = new float[1];

        motorStrength = genome.motorStrength;
        angleSensitivity = genome.angleSensitivity;
        useX = genome.useX;
        useY = genome.useY;
        useZ = genome.useZ;

        MeasureRestAngles();
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (useX) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = throttleX;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
                if (nid.neuronID == 3) {
                    neuron.currentValue = angleX;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
            }
            if (useY) {
                if (nid.neuronID == 1) {
                    neuron.currentValue = throttleY;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
                if (nid.neuronID == 4) {
                    neuron.currentValue = angleY;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
            }
            if (useZ) {
                if (nid.neuronID == 2) {
                    neuron.currentValue = throttleZ;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
                if (nid.neuronID == 5) {
                    neuron.currentValue = angleZ;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
            }
        }
    }

    public void Tick() {
        joint.targetAngularVelocity = new Vector3(throttleX[0] * motorStrength, throttleY[0] * motorStrength, throttleZ[0] * motorStrength);

        MeasureJointAngles();
    }

    private void MeasureRestAngles() {                   
        restRotation = Quaternion.Inverse(joint.connectedBody.GetComponent<Rigidbody>().transform.rotation) * joint.gameObject.GetComponent<Rigidbody>().transform.rotation;
        //Debug.Log(restRotation.ToString());
    }

    private void MeasureJointAngles() {
        
        //float prevAngleX = angleX[0];
        //float prevAngleY = angleY[0];
        //float prevAngleZ = angleZ[0];

        Quaternion currentRotation = Quaternion.Inverse(joint.connectedBody.GetComponent<Rigidbody>().transform.rotation) * joint.gameObject.GetComponent<Rigidbody>().transform.rotation;
        Quaternion DeltaRotation = Quaternion.Inverse(restRotation) * currentRotation;
        Vector3 eulerAngles = DeltaRotation.eulerAngles;
        //Debug.Log(eulerAngles.x.ToString());
        //Debug.Log("TimeStep: " + gameCurrentTimeStep.ToString() + " DeltaRotation: ( " + DeltaRotation.ToString() + " )");
        if (eulerAngles.x > 180f)
            eulerAngles.x -= 360f;
        else if (eulerAngles.x < -180f)
            eulerAngles.x += 360f;
        if (eulerAngles.y > 180f)
            eulerAngles.y -= 360f;
        else if (eulerAngles.y < -180f)
            eulerAngles.y += 360f;
        if (eulerAngles.z > 180f)
            eulerAngles.z -= 360f;
        else if (eulerAngles.z < -180f)
            eulerAngles.z += 360f;
        eulerAngles.x /= Mathf.Rad2Deg;
        eulerAngles.y /= Mathf.Rad2Deg;
        eulerAngles.z /= Mathf.Rad2Deg;
        //Debug.Log("TimeStep: " + gameCurrentTimeStep.ToString() + " JointAngles[" + angleSensor.segmentID.ToString() + "]: ( " + eulerAngles.x.ToString() + ", " + eulerAngles.y.ToString() + ", " + eulerAngles.z.ToString() + " )");
        //Debug.Log(eulerAngles.x.ToString());
        angleX[0] = eulerAngles.x * angleSensitivity;
        angleY[0] = eulerAngles.y * angleSensitivity;
        angleZ[0] = eulerAngles.z * angleSensitivity;

        // Get angle delta:
        /*if (angleSensor.measureVel) {
            angleSensor.angleVelX[0] = (angleSensor.angleX[0] - prevAngleX) * 10f;
            angleSensor.angleVelY[0] = (angleSensor.angleY[0] - prevAngleY) * 10f;
            angleSensor.angleVelZ[0] = (angleSensor.angleZ[0] - prevAngleZ) * 10f;
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicJoint : AgentModuleBase {

    //public int parentID;
    //public int inno;
    public ConfigurableJoint joint;
    private Vector3 restPosition;
    private Quaternion restRotation;
    public float motorStrength;
    public float angleSensitivity;
    // Neurons:
    public float[] throttleX;
    public float[] throttleY;
    public float[] throttleZ;
    public float[] angleX;
    public float[] angleY;
    public float[] angleZ;
    public float[] velX;
    public float[] velY;
    public float[] velZ;
    public float[] posX;
    public float[] posY;
    public float[] posZ;
    public float[] quatX;
    public float[] quatY;
    public float[] quatZ;
    public float[] quatW;

    public bool useX;
    public bool useY;
    public bool useZ;
    public bool angleSensors;
    public bool velocitySensors;
    public bool positionSensors;
    public bool quaternionSensors;

    public BasicJoint() {
        //parentID = genome.parentID;
        //inno = genome.inno;
        //throttle = new float[1];
        //strafe = new float[1];
        //throttle[0] = 0f;
    }

    public void Initialize(BasicJointGenome genome, Agent agent) {        
        parentID = genome.parentID;
        inno = genome.inno;
        motorStrength = genome.motorStrength;
        angleSensitivity = genome.angleSensitivity;
        useX = genome.useX;
        useY = genome.useY;
        useZ = genome.useZ;
        angleSensors = genome.angleSensors;
        velocitySensors = genome.velocitySensors;
        positionSensors = genome.positionSensors;
        quaternionSensors = genome.quaternionSensors;
        isVisible = agent.isVisible;

        

        throttleX = new float[1];
        throttleY = new float[1];
        throttleZ = new float[1];
        if (useX) {            
            if (angleSensors) {
                //Debug.Log("Init Joint AngleX! " + inno.ToString());
                angleX = new float[1];
            }
            if(velocitySensors) {
                velX = new float[1];
            }
        }
        if (useY) {            
            if (angleSensors) {
                angleY = new float[1];
            }
            if (velocitySensors) {
                velY = new float[1];
            }
        }
        if (useZ) {            
            if (angleSensors) {
                angleZ = new float[1];
            }
            if (velocitySensors) {
                velZ = new float[1];
            }
        }
        if(positionSensors) {
            posX = new float[1];
            posY = new float[1];
            posZ = new float[1];
        }
        if (quaternionSensors) {
            quatX = new float[1];
            quatY = new float[1];
            quatZ = new float[1];
            quatW = new float[1];
        }

        joint = agent.segmentList[parentID].GetComponent<ConfigurableJoint>();
        if(joint == null) {
            Debug.LogAssertion("No existing ConfigurableJoint on segment " + parentID.ToString());
        }
                
        MeasureRestState(agent);
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            //Debug.Log("Inno Match!" + nid.moduleID.ToString() + ", " + nid.neuronID.ToString());
            if (useX) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = throttleX;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
                // Extra if() wrappers necessary???
                if(angleSensors) {
                    if (nid.neuronID == 3) {
                        //Debug.Log("Mapped AngleX!");
                        neuron.currentValue = angleX;
                        neuron.neuronType = NeuronGenome.NeuronType.In;
                    }
                }
                if(velocitySensors) {
                    if (nid.neuronID == 6) {
                        neuron.currentValue = velX;
                        neuron.neuronType = NeuronGenome.NeuronType.In;
                    }
                }
            }
            if (useY) {
                if (nid.neuronID == 1) {
                    neuron.currentValue = throttleY;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }                
                if (angleSensors) {
                    if (nid.neuronID == 4) {
                        neuron.currentValue = angleY;
                        neuron.neuronType = NeuronGenome.NeuronType.In;
                    }
                }
                if (velocitySensors) {
                    if (nid.neuronID == 7) {
                        neuron.currentValue = velY;
                        neuron.neuronType = NeuronGenome.NeuronType.In;
                    }
                }
            }
            if (useZ) {
                if (nid.neuronID == 2) {
                    neuron.currentValue = throttleZ;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }                
                if (angleSensors) {
                    if (nid.neuronID == 5) {
                        neuron.currentValue = angleZ;
                        neuron.neuronType = NeuronGenome.NeuronType.In;
                    }
                }
                if (velocitySensors) {
                    if (nid.neuronID == 8) {
                        neuron.currentValue = velZ;
                        neuron.neuronType = NeuronGenome.NeuronType.In;
                    }
                }
            }
            if(positionSensors) {
                if (nid.neuronID == 9) {
                    neuron.currentValue = posX;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 10) {
                    neuron.currentValue = posY;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 11) {
                    neuron.currentValue = posZ;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
            }
            if(quaternionSensors) {
                if (nid.neuronID == 12) {
                    neuron.currentValue = quatX;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 13) {
                    neuron.currentValue = quatY;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 14) {
                    neuron.currentValue = quatZ;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 15) {
                    neuron.currentValue = quatW;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
            }
        }
    }

    public void Tick(Agent agent) {
        joint.targetAngularVelocity = new Vector3(throttleX[0] * motorStrength, throttleY[0] * motorStrength, throttleZ[0] * motorStrength);

        MeasureJointAngles(agent);
    }

    private void MeasureRestState(Agent agent) {                   
        restRotation = Quaternion.Inverse(joint.connectedBody.GetComponent<Rigidbody>().transform.rotation) * joint.gameObject.GetComponent<Rigidbody>().transform.rotation;
        restPosition = agent.segmentList[0].transform.InverseTransformPoint(joint.gameObject.GetComponent<Rigidbody>().transform.position);
        //Debug.Log(restRotation.ToString());
    }

    private void MeasureJointAngles(Agent agent) {

        float prevAngleX = 0f;
        float prevAngleY = 0f;
        float prevAngleZ = 0f;
        if (angleSensors && velocitySensors) {
            if (useX) {
                prevAngleX = angleX[0];
            }
            if (useY) {
                prevAngleY = angleY[0];
            }
            if (useZ) {
                prevAngleZ = angleZ[0];
            }
        }        

        Quaternion currentRotation = Quaternion.Inverse(joint.connectedBody.GetComponent<Rigidbody>().transform.rotation) * joint.gameObject.GetComponent<Rigidbody>().transform.rotation;
        Quaternion deltaRotation = Quaternion.Inverse(restRotation) * currentRotation;
        Vector3 eulerAngles = deltaRotation.eulerAngles;
        if (quaternionSensors) {
            quatX[0] = deltaRotation.x;
            quatY[0] = deltaRotation.y;
            quatZ[0] = deltaRotation.z;
            quatW[0] = deltaRotation.w;
        }
        Vector3 currentPosition = agent.segmentList[0].transform.InverseTransformPoint(joint.gameObject.GetComponent<Rigidbody>().transform.position);
        Vector3 deltaPosition = currentPosition - restPosition;
        if (positionSensors) {
            posX[0] = deltaPosition.x;
            posY[0] = deltaPosition.y;
            posZ[0] = deltaPosition.z;
        }

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
        if(angleSensors) {
            if (useX) {
                angleX[0] = eulerAngles.x * angleSensitivity;
            }
            if (useY) {
                angleY[0] = eulerAngles.y * angleSensitivity;
            }
            if (useZ) {
                angleZ[0] = eulerAngles.z * angleSensitivity;
            }

            // Get angle delta:
            if (velocitySensors) {
                if (useX) {
                    velX[0] = (angleX[0] - prevAngleX) * 1f;
                }
                if (useY) {
                    velY[0] = (angleY[0] - prevAngleY) * 1f;
                }
                if (useZ) {
                    velZ[0] = (angleZ[0] - prevAngleZ) * 1f;
                }                
            }
        }        
    }
}

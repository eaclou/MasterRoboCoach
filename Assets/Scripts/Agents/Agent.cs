using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //public AgentGenome genome;
    public Brain brain;

    public List<GameObject> segmentList;

    public List<InputValue> inputValueList;
    public List<ContactSensor> contactSensorList;
    public List<TargetSensor> targetSensorList;
    public List<RaycastSensor> raycastSensorList;
    public List<ThrusterEffector> thrusterEffectorList;
    public List<TorqueEffector> torqueEffectorList;

	// Use this for initialization
	void Start () {
        //Debug.Log("New Agent!");
    }

    public void MapNeuronToModule(NID nid, Neuron neuron) {
        for (int i = 0; i < inputValueList.Count; i++) {
            if(inputValueList[i].inno == nid.moduleID) {
                //Debug.Log("neuron match!!! inputValue");
                neuron.currentValue = inputValueList[i].value;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
        for (int i = 0; i < targetSensorList.Count; i++) {
            if (targetSensorList[i].inno == nid.moduleID) {
                if(nid.neuronID == 0) {
                    //Debug.Log("neuron match!!! targetSensorX");
                    neuron.currentValue = targetSensorList[i].dotX;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 1) {
                    //Debug.Log("neuron match!!! targetSensorY");
                    neuron.currentValue = targetSensorList[i].dotY;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 2) {
                    //Debug.Log("neuron match!!! targetSensorZ");
                    neuron.currentValue = targetSensorList[i].dotZ;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 3) {
                    neuron.currentValue = targetSensorList[i].vel;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 4) {
                    neuron.currentValue = targetSensorList[i].angVel;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 5) {
                    neuron.currentValue = targetSensorList[i].inTarget;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
            }
        }
        for (int i = 0; i < raycastSensorList.Count; i++) {
            if (raycastSensorList[i].inno == nid.moduleID) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = raycastSensorList[i].distanceLeft;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 1) {
                    neuron.currentValue = raycastSensorList[i].distanceLeftCenter;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 2) {
                    neuron.currentValue = raycastSensorList[i].distanceCenter;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 3) {
                    neuron.currentValue = raycastSensorList[i].distanceRightCenter;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 4) {
                    neuron.currentValue = raycastSensorList[i].distanceRight;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 5) {
                    neuron.currentValue = raycastSensorList[i].distanceBack;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 6) {
                    neuron.currentValue = raycastSensorList[i].distanceCenterShort;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
            }
        }
        for (int i = 0; i < thrusterEffectorList.Count; i++) {
            if (thrusterEffectorList[i].inno == nid.moduleID) {
                //Debug.Log("neuron match!!! thrusterEffector");
                if (nid.neuronID == 0) {
                    neuron.currentValue = thrusterEffectorList[i].throttle;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
                if (nid.neuronID == 1) {
                    neuron.currentValue = thrusterEffectorList[i].strafe;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
            }
        }
        for (int i = 0; i < torqueEffectorList.Count; i++) {
            if (torqueEffectorList[i].inno == nid.moduleID) {
                //Debug.Log("neuron match!!! torqueEffector");
                neuron.currentValue = torqueEffectorList[i].throttle;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
        }
        // Hidden:
        //for(int i = 0; i < genome.bra)
    }

    public void TickBrain() {
        //for(int i = 0; i < brain.neuronList.Count; i++) {
        //brain.neuronList[i].currentValue[0] = 1f;
        //}
        brain.BrainMasterFunction();
        //brain.PrintBrain();
    }
    public void RunModules() {
        //for (int i = 0; i < inputValueList.Count; i++) {            
        //}
        for (int i = 0; i < targetSensorList.Count; i++) {  
            // NORMALIZED!!!!
            Vector3 segmentToTargetVect = new Vector3(targetSensorList[i].targetPosition.position.x - this.segmentList[0].transform.position.x, targetSensorList[i].targetPosition.position.y - this.segmentList[0].transform.position.y, targetSensorList[i].targetPosition.position.z - this.segmentList[0].transform.position.z);
            Vector3 segmentToTargetVectNormalized = segmentToTargetVect.normalized;
            Vector3 rightVector;
            Vector3 upVector;
            Vector3 forwardVector;

            rightVector = segmentList[targetSensorList[i].parentID].transform.right;
            upVector = segmentList[targetSensorList[i].parentID].transform.up;
            forwardVector = segmentList[targetSensorList[i].parentID].transform.forward;

            // Not Normalized!
            float dotRight = Vector3.Dot(segmentToTargetVectNormalized, rightVector);
            float dotUp = Vector3.Dot(segmentToTargetVectNormalized, upVector);
            float dotForward = Vector3.Dot(segmentToTargetVectNormalized, forwardVector);

            targetSensorList[i].dotX[0] = dotRight;
            targetSensorList[i].dotY[0] = dotUp;
            targetSensorList[i].dotZ[0] = dotForward;

            //relative = transform.InverseTransformDirection(0, 0, 1);
            targetSensorList[i].vel[0] = Vector3.Dot(segmentToTargetVect, rightVector);
            targetSensorList[i].angVel[0] = Vector3.Dot(segmentToTargetVect, forwardVector);
            float inTarget = 0f;
            if (segmentToTargetVect.magnitude <= 2f) { // hardcoded radius
                inTarget = 1f;
            }
            targetSensorList[i].inTarget[0] = inTarget;
        }

        for(int i = 0; i < raycastSensorList.Count; i++) {
            Vector3 rayOrigin = this.segmentList[0].GetComponent<Rigidbody>().transform.position + new Vector3(0f, 0.5f, 0f);
            Vector3 left = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(-1f, 0f, 0.6f).normalized);
            Vector3 leftCenter = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(-0.6f, 0f, 1f).normalized);
            Vector3 center = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, 1f));
            Vector3 rightCenter = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(.6f, 0f, 1f).normalized);
            Vector3 right = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(1f, 0f, 0.6f).normalized);
            Vector3 back = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, -1f));
            //Vector3 centerShort = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, -1f));

            RaycastHit hit;

            float rayMaxDistance = 5f;
            float sensitivity = 1f / rayMaxDistance;
            //Debug.Log("raycastSensorList: ");
            raycastSensorList[i].distanceLeft[0] = rayMaxDistance * sensitivity;
            raycastSensorList[i].distanceLeftCenter[0] = rayMaxDistance * sensitivity;
            raycastSensorList[i].distanceCenter[0] = rayMaxDistance * sensitivity;
            raycastSensorList[i].distanceRightCenter[0] = rayMaxDistance * sensitivity;
            raycastSensorList[i].distanceRight[0] = rayMaxDistance * sensitivity;
            raycastSensorList[i].distanceBack[0] = rayMaxDistance * sensitivity;
            raycastSensorList[i].distanceCenterShort[0] = rayMaxDistance * sensitivity;

            if (Physics.Raycast(rayOrigin, left, out hit, rayMaxDistance)) {
                raycastSensorList[i].distanceLeft[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, leftCenter, out hit, rayMaxDistance)) {
                raycastSensorList[i].distanceLeftCenter[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance * 3f)) {
                raycastSensorList[i].distanceCenter[0] = hit.distance * sensitivity / 3f;
            }
            if (Physics.Raycast(rayOrigin, rightCenter, out hit, rayMaxDistance)) {
                raycastSensorList[i].distanceRightCenter[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, right, out hit, rayMaxDistance)) {
                raycastSensorList[i].distanceRight[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, back, out hit, rayMaxDistance)) {
                raycastSensorList[i].distanceBack[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance)) {
                raycastSensorList[i].distanceCenterShort[0] = hit.distance * sensitivity;
            }

            raycastSensorList[i].distanceLeft[0] = 1f - raycastSensorList[i].distanceLeft[0];
            raycastSensorList[i].distanceLeftCenter[0] = 1f - raycastSensorList[i].distanceLeftCenter[0];
            raycastSensorList[i].distanceCenter[0] = 1f - raycastSensorList[i].distanceCenter[0];
            raycastSensorList[i].distanceRightCenter[0] = 1f - raycastSensorList[i].distanceRightCenter[0];
            raycastSensorList[i].distanceRight[0] = 1f - raycastSensorList[i].distanceRight[0];
            raycastSensorList[i].distanceBack[0] = 1f - raycastSensorList[i].distanceBack[0];
            raycastSensorList[i].distanceCenterShort[0] = 1f - raycastSensorList[i].distanceCenterShort[0];
        }
        
        for (int i = 0; i < thrusterEffectorList.Count; i++) {
            //Debug.Log("RunModules! AddRelativeForce " + segmentList[thrusterEffectorList[i].parentID].name);
            //Debug.Log("thruster neuron value: " + thrusterEffectorList[i].throttle[0].ToString());
            segmentList[thrusterEffectorList[i].parentID].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, thrusterEffectorList[i].throttle[0]) * 200f, ForceMode.Force);
            segmentList[thrusterEffectorList[i].parentID].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(thrusterEffectorList[i].strafe[0], 0f, 0f) * 20f, ForceMode.Force);
        }
        for (int i = 0; i < torqueEffectorList.Count; i++) {
            segmentList[torqueEffectorList[i].parentID].GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0f, torqueEffectorList[i].throttle[0], 0f) * 10f, ForceMode.Force);
        }
    }

    public void ConstructAgentFromGenome(AgentGenome genome) {                
        // Destroy existing shit: -- Clean this up later!!! memory overhead!
        var children = new List<GameObject>();
        foreach (Transform child in gameObject.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        if (segmentList == null) {
            segmentList = new List<GameObject>();
        }
        else {
            segmentList.Clear();            
        }

        // Construct Modules:
        inputValueList = new List<InputValue>();
        // TEMP:
        contactSensorList = new List<ContactSensor>();        
        targetSensorList = new List<TargetSensor>();
        raycastSensorList = new List<RaycastSensor>();
        thrusterEffectorList = new List<ThrusterEffector>();
        torqueEffectorList = new List<TorqueEffector>();
        for(int i = 0; i < genome.valueInputList.Count; i++) {
            InputValue inputValue = new InputValue(genome.valueInputList[i]);
            inputValueList.Add(inputValue);
        }
        for (int i = 0; i < genome.targetSensorList.Count; i++) {
            TargetSensor targetSensor = new TargetSensor(genome.targetSensorList[i]);
            targetSensorList.Add(targetSensor);
        }
        for (int i = 0; i < genome.raycastSensorList.Count; i++) {
            RaycastSensor raycastSensor = new RaycastSensor(genome.raycastSensorList[i]);
            raycastSensorList.Add(raycastSensor);
        }
        for (int i = 0; i < genome.thrusterList.Count; i++) {
            ThrusterEffector thrusterEffector = new ThrusterEffector(genome.thrusterList[i]);
            thrusterEffectorList.Add(thrusterEffector);
        }
        for (int i = 0; i < genome.torqueList.Count; i++) {
            TorqueEffector torqueEffector = new TorqueEffector(genome.torqueList[i]);
            torqueEffectorList.Add(torqueEffector);
        }

        // Construct Body:
        for (int i = 0; i < genome.segmentList.Count; i++) {
            //Class clone = (Class)Instantiate('class object', transform.position, transform.rotation);
            GameObject segmentGO = GetSegmentPreset(genome.segmentList[i].segmentPreset); // GameObject.CreatePrimitive(PrimitiveType.Cube); //Instantiate(Resources.Load("robotMesh02")) as GameObject;
            segmentList.Add(segmentGO);
            //segmentGO.GetComponent<MeshFilter>().sharedMesh = genome.segmentList[i].segmentMesh;
            //segmentGO.GetComponent<BoxCollider>().center = new Vector3(0f, 0.5f, 0f);
            segmentGO.name = "segment" + i.ToString();
            segmentGO.transform.parent = gameObject.transform;
            segmentGO.transform.localScale = genome.segmentList[i].scale;
            segmentGO.transform.localPosition = new Vector3(0f, 0f, 0f);
            segmentGO.AddComponent<Rigidbody>().drag = 5f;
            segmentGO.GetComponent<Rigidbody>().angularDrag = 5f;

            ContactSensor contactSensor = segmentGO.AddComponent<ContactSensor>();
            contactSensorList.Add(contactSensor);
        }

        // Construct Brain:
        brain = new Brain(genome.brainGenome, this);
    }

    private GameObject GetSegmentPreset(CustomMeshID meshID) {
        GameObject preset = null;
        switch (meshID) {
            case CustomMeshID.Default:
                Debug.Log("default");
                break;
            case CustomMeshID.Test:
                //Debug.Log("test mesh");
                preset = Instantiate(Resources.Load("SegmentPresets/test")) as GameObject;
                break;
            default:
                Debug.Log("error default");
                break;
        }
        return preset;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //public AgentGenome genome;
    public Brain brain;

    public List<GameObject> segmentList;

    [SerializeField]
    public List<InputValue> inputValueList;
    public List<ContactSensor> contactSensorList;
    public List<HealthModule> healthModuleList;
    public List<TargetSensor> targetSensorList;
    public List<RaycastSensor> raycastSensorList;
    public List<ThrusterEffector> thrusterEffectorList;
    public List<TorqueEffector> torqueEffectorList;
    public List<WeaponProjectile> weaponProjectileList;
    public List<WeaponTazer> weaponTazerList;

    //public Vector3 debugProjectileSource;
    //public Vector3 debugProjectileEnd;
    //public bool debugProjectileOn;
    //public Vector3 debugTazerSource;
    //public Vector3 debugTazerEnd;
    //public bool debugTazerOn;

    public bool isVisible = false;

    // Use this for initialization
    void Start () {
        //Debug.Log("New Agent!");
    }

    private void OnDrawGizmos() {
        /*if(debugProjectileOn) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(debugProjectileSource, debugProjectileEnd);
        }
        if(debugTazerOn) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(debugTazerSource, debugTazerEnd);
        }*/
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
                    //Debug.Log("neuron match!!! targetSensorZ");
                    neuron.currentValue = targetSensorList[i].dotZ;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 2) {
                    neuron.currentValue = targetSensorList[i].forward;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 3) {
                    neuron.currentValue = targetSensorList[i].horizontal;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 4) {
                    neuron.currentValue = targetSensorList[i].inTarget;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 5) {
                    neuron.currentValue = targetSensorList[i].velX;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 6) {
                    neuron.currentValue = targetSensorList[i].velZ;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 7) {
                    neuron.currentValue = targetSensorList[i].targetHealth;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 8) {
                    neuron.currentValue = targetSensorList[i].targetAttacking;
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
        for (int i = 0; i < weaponProjectileList.Count; i++) {
            if (weaponProjectileList[i].inno == nid.moduleID) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = weaponProjectileList[i].energy;
                    neuron.neuronType = NeuronGenome.NeuronType.In;                    
                }
                if (nid.neuronID == 1) {
                    neuron.currentValue = weaponProjectileList[i].throttle;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }                
            }
        }
        for (int i = 0; i < weaponTazerList.Count; i++) {
            if (weaponTazerList[i].inno == nid.moduleID) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = weaponTazerList[i].energy;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 1) {
                    neuron.currentValue = weaponTazerList[i].throttle;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }
            }
        }
        for (int i = 0; i < healthModuleList.Count; i++) {
            if (healthModuleList[i].inno == nid.moduleID) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = healthModuleList[i].healthSensor;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 1) {
                    neuron.currentValue = healthModuleList[i].takingDamage;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
            }
        }
        for (int i = 0; i < contactSensorList.Count; i++) {
            if (contactSensorList[i].inno == nid.moduleID) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = contactSensorList[i].contactSensor;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
            }
        }
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
            Vector3 forwardVector;

            rightVector = segmentList[targetSensorList[i].parentID].transform.right;
            forwardVector = segmentList[targetSensorList[i].parentID].transform.forward;

            // Not Normalized!
            float dotRight = Vector3.Dot(segmentToTargetVectNormalized, rightVector);
            float dotForward = Vector3.Dot(segmentToTargetVectNormalized, forwardVector);

            targetSensorList[i].dotX[0] = dotRight;
            targetSensorList[i].dotZ[0] = dotForward;

            //relative = transform.InverseTransformDirection(0, 0, 1);
            targetSensorList[i].forward[0] = Vector3.Dot(segmentToTargetVect, forwardVector) * targetSensorList[i].sensitivity;
            targetSensorList[i].horizontal[0] = Vector3.Dot(segmentToTargetVect, rightVector) * targetSensorList[i].sensitivity;
            float inTarget = 0f;
            if (segmentToTargetVect.sqrMagnitude <= 12f) { // hardcoded radius
                inTarget = 1f; // ?? better to just give inverse distance??
            }
            targetSensorList[i].inTarget[0] = inTarget;

            Vector3 relativeVelocity;
            if (targetSensorList[i].targetPosition.gameObject.GetComponent<Rigidbody>()) {
                relativeVelocity = targetSensorList[i].targetPosition.gameObject.GetComponent<Rigidbody>().velocity - this.segmentList[0].gameObject.GetComponent<Rigidbody>().velocity;
                Agent agentOpponent = targetSensorList[i].targetPosition.gameObject.transform.parent.GetComponent<Agent>();
                float attacking = 0f;
                if(agentOpponent.weaponProjectileList[0].throttle[0] > 0 && agentOpponent.weaponProjectileList[0].energy[0] >= 0.1f) {
                    attacking = 1f;
                }
                if (agentOpponent.weaponTazerList[0].throttle[0] > 0 && agentOpponent.weaponTazerList[0].energy[0] >= 0.01f) {
                    attacking = 1f;
                }
                targetSensorList[i].targetAttacking[0] = attacking;
            }
            else {
                relativeVelocity = this.segmentList[0].gameObject.GetComponent<Rigidbody>().velocity;
            }
            targetSensorList[i].velX[0] = relativeVelocity.x * targetSensorList[i].sensitivity;
            targetSensorList[i].velZ[0] = relativeVelocity.z * targetSensorList[i].sensitivity;
                        
            if (targetSensorList[i].targetPosition.gameObject.GetComponent<HealthModuleComponent>()) {
                targetSensorList[i].targetHealth[0] = targetSensorList[i].targetPosition.gameObject.GetComponent<HealthModuleComponent>().healthModule.health / targetSensorList[i].targetPosition.gameObject.GetComponent<HealthModuleComponent>().healthModule.maxHealth;
            }
        }

        for(int i = 0; i < raycastSensorList.Count; i++) {
            Vector3 rayOrigin = this.segmentList[0].GetComponent<Rigidbody>().transform.position + new Vector3(0f, 0.5f, 0f);
            Vector3 left = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(-1f, 0f, 0f).normalized);
            Vector3 leftCenter = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(-1f, 0f, 1f).normalized);
            Vector3 center = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, 1f));
            Vector3 rightCenter = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(1f, 0f, 1f).normalized);
            Vector3 right = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(1f, 0f, 0f).normalized);
            Vector3 back = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, -1f));
            //Vector3 centerShort = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, -1f));

            RaycastHit hit;

            float rayMaxDistance = 10f;
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
                if(hit.collider.tag == "hazard")
                    raycastSensorList[i].distanceLeft[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, leftCenter, out hit, rayMaxDistance)) {
                if (hit.collider.tag == "hazard")
                    raycastSensorList[i].distanceLeftCenter[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance * 3f)) {
                if (hit.collider.tag == "hazard")
                    raycastSensorList[i].distanceCenter[0] = hit.distance * sensitivity / 3f;
            }
            if (Physics.Raycast(rayOrigin, rightCenter, out hit, rayMaxDistance)) {
                if (hit.collider.tag == "hazard")
                    raycastSensorList[i].distanceRightCenter[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, right, out hit, rayMaxDistance)) {
                if (hit.collider.tag == "hazard")
                    raycastSensorList[i].distanceRight[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, back, out hit, rayMaxDistance)) {
                if (hit.collider.tag == "hazard")
                    raycastSensorList[i].distanceBack[0] = hit.distance * sensitivity;
            }
            if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance)) {
                if (hit.collider.tag == "hazard")
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
            segmentList[thrusterEffectorList[i].parentID].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0.01f, thrusterEffectorList[i].throttle[0]) * 9000f, ForceMode.Force);
            segmentList[thrusterEffectorList[i].parentID].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(thrusterEffectorList[i].strafe[0], 0f, 0f) * 0f, ForceMode.Force);
        }
        for (int i = 0; i < torqueEffectorList.Count; i++) {
            segmentList[torqueEffectorList[i].parentID].GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0f, torqueEffectorList[i].throttle[0], 0f) * 0f, ForceMode.Force);
        }

        for (int i = 0; i < weaponProjectileList.Count; i++) {
            
            float rayMaxDistance = 30f;
            weaponProjectileList[i].damageInflicted[0] = 0f;

            if(isVisible) {
                ParticleSystem.EmissionModule emission = weaponProjectileList[i].particles.emission;
                emission.enabled = false;
            }

            if (weaponProjectileList[i].throttle[0] > 0f) {
                if (weaponProjectileList[i].energy[0] > 0.01f) {
                    weaponProjectileList[i].energy[0] -= 0.01f; // costs energy to fire

                    Vector3 rayOrigin = this.segmentList[0].GetComponent<Rigidbody>().transform.position + new Vector3(0f, 0.5f, 0f);
                    Vector3 center = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, 1f));
                    RaycastHit hit;
                    //debugProjectileEnd = rayOrigin + center.normalized * rayMaxDistance;
                    if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance)) {
                        if (hit.collider.GetComponent<HealthModuleComponent>() != null) {
                            hit.collider.GetComponent<HealthModuleComponent>().TakeDamage(5f);
                            weaponProjectileList[i].damageInflicted[0] = 5f;
                            //debugProjectileEnd = rayOrigin + center.normalized * hit.distance;                            
                        }
                    }
                    else {

                    }
                    //debugProjectileOn = true;
                    //debugProjectileSource = rayOrigin;
                    //debugProjectileEnd = rayOrigin + center.normalized * rayMaxDistance;

                    if(isVisible) {                        
                        ParticleSystem.EmissionModule emission = weaponProjectileList[i].particles.emission;
                        emission.enabled = true;                        
                    }
                }
            }
            else {
                weaponProjectileList[i].energy[0] += 0.01f;
            }
        }
        for (int i = 0; i < weaponTazerList.Count; i++) {
            
            float rayMaxDistance = 5f;
            weaponTazerList[i].damageInflicted[0] = 0f;

            if (isVisible) {
                ParticleSystem.EmissionModule emission = weaponTazerList[i].particles.emission;
                emission.enabled = false;
            }

            if (weaponTazerList[i].throttle[0] > 0f) {
                if (weaponTazerList[i].energy[0] > 0.1f) {
                    weaponTazerList[i].energy[0] -= 0.1f; // costs energy to fire
                    
                    Vector3 rayOrigin = this.segmentList[0].GetComponent<Rigidbody>().transform.position + new Vector3(0f, 0.5f, 0f);
                    Vector3 center = this.segmentList[0].GetComponent<Rigidbody>().transform.TransformDirection(new Vector3(0f, 0f, 1f));
                    RaycastHit hit;                    
                    if (Physics.Raycast(rayOrigin, center, out hit, rayMaxDistance)) {
                        if (hit.collider.GetComponent<HealthModuleComponent>() != null) {
                            hit.collider.GetComponent<HealthModuleComponent>().TakeDamage(25f);
                            weaponTazerList[i].damageInflicted[0] = 25f;
                        }
                    }

                    if (isVisible) {
                        ParticleSystem.EmissionModule emission = weaponTazerList[i].particles.emission;
                        emission.enabled = true;
                    }
                }
            }
            else {
                weaponTazerList[i].energy[0] += 0.01f;
            }
        }
        for (int i = 0; i < healthModuleList.Count; i++) {
            healthModuleList[i].healthSensor[0] = healthModuleList[i].health / healthModuleList[i].maxHealth;
            if(healthModuleList[i].health != healthModuleList[i].prevHealth) {
                //healthModuleList[i].takingDamage[0] = 1f;                
            }
            else {
                //healthModuleList[i].takingDamage[0] = 0f;                
            }
            //healthModuleList[i].prevHealth = healthModuleList[i].health;
            healthModuleList[i].takingDamage[0] = (healthModuleList[i].maxHealth - healthModuleList[i].health) / healthModuleList[i].maxHealth;
        }
        for (int i = 0; i < contactSensorList.Count; i++) {
            float contact = 0f;
            if(contactSensorList[i].component.contact) {
                contact = 1f;
            }
            contactSensorList[i].contactSensor[0] = contact;
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
            segmentGO.transform.localRotation = Quaternion.identity;
            //segmentGO.AddComponent<Rigidbody>().drag = 5f;
            //segmentGO.GetComponent<Rigidbody>().angularDrag = 5f;
        }

        // Construct Modules:
        inputValueList = new List<InputValue>();
        
        contactSensorList = new List<ContactSensor>();
        healthModuleList = new List<HealthModule>();

        targetSensorList = new List<TargetSensor>();
        raycastSensorList = new List<RaycastSensor>();
        thrusterEffectorList = new List<ThrusterEffector>();
        torqueEffectorList = new List<TorqueEffector>();
        weaponProjectileList = new List<WeaponProjectile>();
        weaponTazerList = new List<WeaponTazer>();

        for(int i = 0; i < genome.valueInputList.Count; i++) {
            InputValue inputValue = new InputValue(genome.valueInputList[i]);
            inputValueList.Add(inputValue);
        }

        for (int i = 0; i < genome.healthModuleList.Count; i++) {
            HealthModule healthModule = new HealthModule(genome.healthModuleList[i]);
            healthModuleList.Add(healthModule);
            HealthModuleComponent healthModuleComponent = segmentList[genome.healthModuleList[i].parentID].AddComponent<HealthModuleComponent>();
            healthModuleComponent.healthModule = healthModule; // share ref
            healthModule.component = healthModuleComponent;  // share ref
            healthModuleList.Add(healthModule);            
        }
        for (int i = 0; i < genome.contactSensorList.Count; i++) {
            ContactSensor contactSensor = new ContactSensor(genome.contactSensorList[i]);
            contactSensorList.Add(contactSensor);
            ContactSensorComponent contactSensorComponent = segmentList[genome.contactSensorList[i].parentID].AddComponent<ContactSensorComponent>();
            contactSensor.component = contactSensorComponent;
            contactSensorComponent.sensor = contactSensor;
            contactSensorList.Add(contactSensor);
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
        for (int i = 0; i < genome.weaponProjectileList.Count; i++) {
            WeaponProjectile weaponProjectile = new WeaponProjectile(genome.weaponProjectileList[i]);
            weaponProjectileList.Add(weaponProjectile);
            if (isVisible) {
                GameObject particleGO = Instantiate(Resources.Load(weaponProjectileList[i].GetParticleSystemURL())) as GameObject;
                ParticleSystem particle = particleGO.GetComponent<ParticleSystem>();
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = false;
                particle.gameObject.transform.parent = segmentList[weaponProjectile.parentID].transform;
                particle.gameObject.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                particle.gameObject.transform.localRotation = Quaternion.identity;
                weaponProjectile.particles = particle; // save reference                 
            }
        }
        for (int i = 0; i < genome.weaponTazerList.Count; i++) {
            WeaponTazer weaponTazer = new WeaponTazer(genome.weaponTazerList[i]);
            weaponTazerList.Add(weaponTazer);
            if (isVisible) {
                GameObject particleGO = Instantiate(Resources.Load(weaponTazerList[i].GetParticleSystemURL())) as GameObject;
                ParticleSystem particle = particleGO.GetComponent<ParticleSystem>();
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = false;
                particle.gameObject.transform.parent = segmentList[weaponTazer.parentID].transform;
                particle.gameObject.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                particle.gameObject.transform.localRotation = Quaternion.identity;
                weaponTazer.particles = particle; // save reference                 
            }
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
            case CustomMeshID.CombatBody:
                //Debug.Log("test mesh");
                preset = Instantiate(Resources.Load("SegmentPresets/combatBody")) as GameObject;
                break;
            case CustomMeshID.CombatCar:
                //Debug.Log("test mesh");
                preset = Instantiate(Resources.Load("SegmentPresets/combatCar")) as GameObject;
                break;
            default:
                Debug.Log("error default");
                break;
        }
        return preset;
    }
}

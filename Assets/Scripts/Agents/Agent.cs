using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //public AgentGenome genome;
    public Brain brain;

    //public List<GameObject> segmentList;
    [SerializeField]
    public GameObject rootObject;
    public Vector3 rootCOM;

    [SerializeField]
    public List<GameObject> segmentList; // This is populated primarily at edit-time through the inspector.
    // in the future with more complex module additions, this might be extended programmatically.
    [SerializeField]
    public List<GameObject> visibleObjectList;
        
    [System.NonSerialized]
    public List<BasicWheel> basicWheelList;
    [System.NonSerialized]
    public List<BasicJoint> basicJointList;
    [System.NonSerialized]
    public List<ContactSensor> contactSensorList;
    [System.NonSerialized]
    public List<HealthModule> healthModuleList;
    [System.NonSerialized]
    public List<InputOscillator> oscillatorList;
    [System.NonSerialized]
    public List<RaycastSensor> raycastSensorList;
    [System.NonSerialized]
    public List<TargetSensor> targetSensorList;
    [System.NonSerialized]
    public List<ThrusterEffector> thrusterEffectorList;
    [System.NonSerialized]
    public List<TorqueEffector> torqueEffectorList;
    [System.NonSerialized]
    public List<InputValue> valueList;
    [System.NonSerialized]
    public List<WeaponProjectile> weaponProjectileList;
    [System.NonSerialized]
    public List<WeaponTazer> weaponTazerList;    
    

    public bool isVisible = false;

    // Use this for initialization
    void Start () {
        //Debug.Log("New Agent!");
    }

    public void MapNeuronToModule(NID nid, Neuron neuron) {
        for (int i = 0; i < basicWheelList.Count; i++) {
            basicWheelList[i].MapNeuron(nid, neuron);
        }
        for (int i = 0; i < basicJointList.Count; i++) {
            basicJointList[i].MapNeuron(nid, neuron);
        }
        for (int i = 0; i < contactSensorList.Count; i++) {
            contactSensorList[i].MapNeuron(nid, neuron);
        }
        for (int i = 0; i < healthModuleList.Count; i++) {
            healthModuleList[i].MapNeuron(nid, neuron);
        }        
        for (int i = 0; i < oscillatorList.Count; i++) {
            oscillatorList[i].MapNeuron(nid, neuron);            
        }
        for (int i = 0; i < raycastSensorList.Count; i++) {
            raycastSensorList[i].MapNeuron(nid, neuron);
        }
        for (int i = 0; i < targetSensorList.Count; i++) {
            targetSensorList[i].MapNeuron(nid, neuron);            
        }        
        for (int i = 0; i < thrusterEffectorList.Count; i++) {
            thrusterEffectorList[i].MapNeuron(nid, neuron);            
        }
        for (int i = 0; i < torqueEffectorList.Count; i++) {
            torqueEffectorList[i].MapNeuron(nid, neuron);            
        }
        for (int i = 0; i < valueList.Count; i++) {
            valueList[i].MapNeuron(nid, neuron);
        }
        for (int i = 0; i < weaponProjectileList.Count; i++) {
            weaponProjectileList[i].MapNeuron(nid, neuron);            
        }
        for (int i = 0; i < weaponTazerList.Count; i++) {
            weaponTazerList[i].MapNeuron(nid, neuron);            
        }
        // Hidden nodes!
        if(nid.moduleID == -1) {
            //Debug.Log("Map Hidden Neuron #" + nid.neuronID.ToString());

            neuron.currentValue = new float[1];
            neuron.neuronType = NeuronGenome.NeuronType.Hid;
            neuron.previousValue = 0f;
        }
    }

    public void TickBrain() {
        //for(int i = 0; i < brain.neuronList.Count; i++) {
        //brain.neuronList[i].currentValue[0] = 1f;
        //}
        brain.BrainMasterFunction();
        //brain.PrintBrain();
    }
    public void RunModules(int timeStep) {
        for (int i = 0; i < basicWheelList.Count; i++) {
            basicWheelList[i].Tick();
        }
        for (int i = 0; i < basicJointList.Count; i++) {
            basicJointList[i].Tick(this); // needed for root segment transform
        }
        for (int i = 0; i < contactSensorList.Count; i++) {
            contactSensorList[i].Tick();
        }
        for (int i = 0; i < healthModuleList.Count; i++) {
            healthModuleList[i].Tick();
        }        
        for (int i = 0; i < oscillatorList.Count; i++) {
            oscillatorList[i].Tick(timeStep);
        }
        for (int i = 0; i < raycastSensorList.Count; i++) {
            raycastSensorList[i].Tick();
        }
        for (int i = 0; i < targetSensorList.Count; i++) {
            targetSensorList[i].Tick();
        }        
        for (int i = 0; i < thrusterEffectorList.Count; i++) {
            thrusterEffectorList[i].Tick();            
        }
        for (int i = 0; i < torqueEffectorList.Count; i++) {
            torqueEffectorList[i].Tick();            
        }
        //for (int i = 0; i < valueList.Count; i++) {            
        //}
        for (int i = 0; i < weaponProjectileList.Count; i++) {
            weaponProjectileList[i].Tick();            
        }
        for (int i = 0; i < weaponTazerList.Count; i++) {
            weaponTazerList[i].Tick();            
        }
    }

    public void InitializeModules(AgentGenome genome, Agent agent) {
        basicWheelList = new List<BasicWheel>();
        basicJointList = new List<BasicJoint>();    
        contactSensorList = new List<ContactSensor>();    
        healthModuleList = new List<HealthModule>();    
        oscillatorList = new List<InputOscillator>();    
        raycastSensorList = new List<RaycastSensor>();    
        targetSensorList = new List<TargetSensor>();    
        thrusterEffectorList = new List<ThrusterEffector>();    
        torqueEffectorList = new List<TorqueEffector>();    
        valueList = new List<InputValue>();    
        weaponProjectileList = new List<WeaponProjectile>();    
        weaponTazerList = new List<WeaponTazer>();

        for (int i = 0; i < genome.bodyGenome.basicWheelList.Count; i++) {
            BasicWheel basicWheel = new BasicWheel();
            basicWheel.Initialize(genome.bodyGenome.basicWheelList[i], agent);
            basicWheelList.Add(basicWheel);
            //OLD:
            //basicWheelList[i].Initialize(genome.basicWheelList[i]);
        }
        for (int i = 0; i < genome.bodyGenome.basicJointList.Count; i++) {
            BasicJoint basicJoint = new BasicJoint();
            basicJoint.Initialize(genome.bodyGenome.basicJointList[i], agent);
            basicJointList.Add(basicJoint);
            //basicJointList[i].Initialize(genome.basicJointList[i]);
        }
        for (int i = 0; i < genome.bodyGenome.contactSensorList.Count; i++) {
            ContactSensor contactSensor = new ContactSensor();
            //agent.segmentList[genome.contactSensorList[i].parentID].AddComponent<ContactSensorComponent>();
            contactSensor.Initialize(genome.bodyGenome.contactSensorList[i], agent);
            contactSensorList.Add(contactSensor);
            
            //contactSensorList[i].Initialize(genome.contactSensorList[i]);
        }
        for (int i = 0; i < genome.bodyGenome.healthModuleList.Count; i++) {
            HealthModule healthModule = new HealthModule();
            //agent.segmentList[genome.healthModuleList[i].parentID].AddComponent<HealthModuleComponent>();
            healthModule.Initialize(genome.bodyGenome.healthModuleList[i], agent);
            healthModuleList.Add(healthModule);
            //healthModuleList[i].Initialize(genome.healthModuleList[i]);
        }        
        for (int i = 0; i < genome.bodyGenome.oscillatorInputList.Count; i++) {
            InputOscillator inputOscillator = new InputOscillator();
            inputOscillator.Initialize(genome.bodyGenome.oscillatorInputList[i], agent);
            oscillatorList.Add(inputOscillator);
            //oscillatorList[i].Initialize(genome.oscillatorInputList[i]);
        }
        for (int i = 0; i < genome.bodyGenome.raycastSensorList.Count; i++) {
            RaycastSensor raycastSensor = new RaycastSensor();
            raycastSensor.Initialize(genome.bodyGenome.raycastSensorList[i], agent);
            raycastSensorList.Add(raycastSensor);
            //raycastSensorList[i].Initialize(genome.raycastSensorList[i]);
        }
        for (int i = 0; i < genome.bodyGenome.targetSensorList.Count; i++) {
            TargetSensor targetSensor = new TargetSensor();
            targetSensor.Initialize(genome.bodyGenome.targetSensorList[i], agent);
            targetSensorList.Add(targetSensor);
            //targetSensorList[i].Initialize(genome.targetSensorList[i]);
        }        
        for (int i = 0; i < genome.bodyGenome.thrusterList.Count; i++) {
            // Create Functional GameObjects & Components:
            // none in this case            
            
            // Create Logic Module:
            ThrusterEffector thrusterEffector = new ThrusterEffector(); 
            // Initialize and HookUp Logic Module:
            thrusterEffector.Initialize(genome.bodyGenome.thrusterList[i], agent);
            // If Visible, Create Renderable GameObjects & Components:
            if (isVisible) {
                // Find appropriate Prefab based on Agent & Module Genome:
                GameObject thrusterGO = Instantiate(Resources.Load("Prefabs/Modules/Thrusters/thrusterTest")) as GameObject;
                thrusterGO.transform.parent = agent.segmentList[genome.bodyGenome.thrusterList[i].parentID].transform;
                thrusterGO.transform.localPosition = genome.bodyGenome.thrusterList[i].forcePoint;
                thrusterGO.transform.localRotation = Quaternion.identity;
                // Hook into Logic Module
                thrusterEffector.thrusterComponent = thrusterGO.GetComponent<ThrusterComponent>();
            }
            // Add Logic Module to Agent's Master List
            thrusterEffectorList.Add(thrusterEffector);
        }
        for (int i = 0; i < genome.bodyGenome.torqueList.Count; i++) {
            TorqueEffector torqueEffector = new TorqueEffector();
            torqueEffector.Initialize(genome.bodyGenome.torqueList[i], agent);
            torqueEffectorList.Add(torqueEffector);
            //torqueEffectorList[i].Initialize(genome.torqueList[i]);
        }
        for (int i = 0; i < genome.bodyGenome.valueInputList.Count; i++) {
            InputValue inputValue = new InputValue();
            inputValue.Initialize(genome.bodyGenome.valueInputList[i], agent);
            valueList.Add(inputValue);
            //valueList[i].Initialize(genome.valueInputList[i]);
        }
        for (int i = 0; i < genome.bodyGenome.weaponProjectileList.Count; i++) {
            WeaponProjectile weaponProjectile = new WeaponProjectile();
            weaponProjectile.Initialize(genome.bodyGenome.weaponProjectileList[i], agent);

            if (isVisible) {
                // Find appropriate Prefab based on Agent & Module Genome:
                GameObject weaponGO = Instantiate(Resources.Load("Prefabs/Modules/WeaponProjectiles/projectileTest")) as GameObject;
                weaponGO.transform.parent = agent.segmentList[genome.bodyGenome.weaponProjectileList[i].parentID].transform;
                weaponGO.transform.localPosition = genome.bodyGenome.weaponProjectileList[i].muzzleLocation;
                weaponGO.transform.localRotation = Quaternion.identity;
                // Hook into Logic Module
                weaponProjectile.weaponProjectileComponent = weaponGO.GetComponent<WeaponProjectileComponent>();
            }

            weaponProjectileList.Add(weaponProjectile);

            /*weaponProjectileList[i].Initialize(genome.weaponProjectileList[i]);

            if (isVisible) {
                GameObject particleGO = Instantiate(Resources.Load(weaponProjectileList[i].GetParticleSystemURL())) as GameObject;
                ParticleSystem particle = particleGO.GetComponent<ParticleSystem>();
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = false;
                particle.gameObject.transform.parent = rootObject.transform;
                particle.gameObject.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                particle.gameObject.transform.localRotation = Quaternion.identity;
                weaponProjectileList[i].particles = particle; // save reference                 
            }
            */
        }
        for (int i = 0; i < genome.bodyGenome.weaponTazerList.Count; i++) {
            WeaponTazer weaponTazer = new WeaponTazer();
            weaponTazer.Initialize(genome.bodyGenome.weaponTazerList[i], agent);

            if (isVisible) {
                // Find appropriate Prefab based on Agent & Module Genome:
                GameObject weaponGO = Instantiate(Resources.Load("Prefabs/Modules/WeaponTazers/tazerTest")) as GameObject;
                weaponGO.transform.parent = agent.segmentList[genome.bodyGenome.weaponTazerList[i].parentID].transform;
                weaponGO.transform.localPosition = genome.bodyGenome.weaponTazerList[i].muzzleLocation;
                weaponGO.transform.localRotation = Quaternion.identity;
                // Hook into Logic Module
                weaponTazer.weaponTazerComponent = weaponGO.GetComponent<WeaponTazerComponent>();
            }

            weaponTazerList.Add(weaponTazer);

            /*weaponTazerList[i].Initialize(genome.weaponTazerList[i]);
            if (isVisible) {
                GameObject particleGO = Instantiate(Resources.Load(weaponTazerList[i].GetParticleSystemURL())) as GameObject;
                ParticleSystem particle = particleGO.GetComponent<ParticleSystem>();
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = false;
                particle.gameObject.transform.parent = weaponTazerList[i].parentBody.transform;
                particle.gameObject.transform.localPosition = weaponTazerList[i].muzzleLocation;
                particle.gameObject.transform.localRotation = Quaternion.identity;
                weaponTazerList[i].particles = particle; // save reference                 
            }
            */
        }
    }

    public void InitializeAgentFromTemplate(AgentGenome genome) {
        // Initialize Modules --
        //Debug.Log("Agent Initialize Modules() segment count: " + segmentList.Count.ToString() + ", visCount: " + visibleObjectList.Count.ToString());
        // -- Setup that used to be done in the constructors
        InitializeModules(genome, this);

        // Visible/Non-Visible:
        if(isVisible) {
            for(int i = 0; i < visibleObjectList.Count; i++) {
                visibleObjectList[i].SetActive(true);
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
                preset = Instantiate(Resources.Load("Prefabs/SegmentPresets/test")) as GameObject;
                break;
            case CustomMeshID.CombatBody:
                //Debug.Log("test mesh");
                preset = Instantiate(Resources.Load("Prefabs/SegmentPresets/combatBody")) as GameObject;
                break;
            case CustomMeshID.CombatCar:
                //Debug.Log("test mesh");
                preset = Instantiate(Resources.Load("Prefabs/SegmentPresets/combatCar")) as GameObject;
                break;
            default:
                Debug.Log("error default");
                break;
        }
        return preset;
    }
}

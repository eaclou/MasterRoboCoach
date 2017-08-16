using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentGenome {

    // Naive Implementation!!! Re-factor later when I have a better idea of how to structure this!!
    // Agent Genome:
    // Brain Genome
    // Module Lists - one per module type - points to parent GameObjectID
    // Segment List -- nodeList
    public int index = -1;

    //public GameObject bodyGO;

    public BrainGenome brainGenome;
    //public List<SegmentGenome> segmentList;
    // Modules:
    public List<BasicWheelGenome> basicWheelList;
    public List<BasicJointGenome> basicJointList;
    public List<ContactGenome> contactSensorList;
    public List<HealthGenome> healthModuleList;
    public List<OscillatorGenome> oscillatorInputList;
    public List<RaycastSensorGenome> raycastSensorList;
    public List<TargetSensorGenome> targetSensorList;    
    public List<ThrusterGenome> thrusterList;
    public List<TorqueGenome> torqueList;
    public List<ValueInputGenome> valueInputList;
    public List<WeaponProjectileGenome> weaponProjectileList;
    public List<WeaponTazerGenome> weaponTazerList;    

    // Constructor
    public AgentGenome(int index) {
        this.index = index;
    }

    public void CopyGenomeFromTemplate(AgentGenome templateGenome) {
        // This method creates a clone of the provided ScriptableObject Genome - should have no shared references!!!
        // copy segment list:
        /*segmentList = new List<SegmentGenome>();
        for(int i = 0; i < templateGenome.segmentList.Count; i++) {
            SegmentGenome segmentCopy = new SegmentGenome(templateGenome.segmentList[i]);
            segmentList.Add(segmentCopy);
        }*/
        // copy module lists:
        basicWheelList = new List<BasicWheelGenome>();
        for (int i = 0; i < templateGenome.basicWheelList.Count; i++) {
            BasicWheelGenome genomeCopy = new BasicWheelGenome(templateGenome.basicWheelList[i]);
            basicWheelList.Add(genomeCopy);
        }
        basicJointList = new List<BasicJointGenome>();
        for (int i = 0; i < templateGenome.basicJointList.Count; i++) {
            BasicJointGenome genomeCopy = new BasicJointGenome(templateGenome.basicJointList[i]);
            basicJointList.Add(genomeCopy);
        }
        contactSensorList = new List<ContactGenome>();
        for (int i = 0; i < templateGenome.contactSensorList.Count; i++) {
            ContactGenome genomeCopy = new ContactGenome(templateGenome.contactSensorList[i]);
            contactSensorList.Add(genomeCopy);
        }
        healthModuleList = new List<HealthGenome>();
        for (int i = 0; i < templateGenome.healthModuleList.Count; i++) {
            HealthGenome genomeCopy = new HealthGenome(templateGenome.healthModuleList[i]);
            healthModuleList.Add(genomeCopy);
        }
        oscillatorInputList = new List<OscillatorGenome>();
        for (int i = 0; i < templateGenome.oscillatorInputList.Count; i++) {
            OscillatorGenome genomeCopy = new OscillatorGenome(templateGenome.oscillatorInputList[i]);
            oscillatorInputList.Add(genomeCopy);
        }
        raycastSensorList = new List<RaycastSensorGenome>();
        for (int i = 0; i < templateGenome.raycastSensorList.Count; i++) {
            RaycastSensorGenome genomeCopy = new RaycastSensorGenome(templateGenome.raycastSensorList[i]);
            raycastSensorList.Add(genomeCopy);
        }
        targetSensorList = new List<TargetSensorGenome>();
        for(int i = 0; i < templateGenome.targetSensorList.Count; i++) {
            TargetSensorGenome genomeCopy = new TargetSensorGenome(templateGenome.targetSensorList[i]);
            targetSensorList.Add(genomeCopy);
        }     
        thrusterList = new List<ThrusterGenome>();
        for (int i = 0; i < templateGenome.thrusterList.Count; i++) {
            ThrusterGenome genomeCopy = new ThrusterGenome(templateGenome.thrusterList[i]);
            thrusterList.Add(genomeCopy);
        }        
        torqueList = new List<TorqueGenome>();
        for (int i = 0; i < templateGenome.torqueList.Count; i++) {
            TorqueGenome genomeCopy = new TorqueGenome(templateGenome.torqueList[i]);
            torqueList.Add(genomeCopy);
        }        
        valueInputList = new List<ValueInputGenome>();
        for (int i = 0; i < templateGenome.valueInputList.Count; i++) {
            ValueInputGenome genomeCopy = new ValueInputGenome(templateGenome.valueInputList[i]);
            valueInputList.Add(genomeCopy);
        }        
        weaponProjectileList = new List<WeaponProjectileGenome>();
        for (int i = 0; i < templateGenome.weaponProjectileList.Count; i++) {
            WeaponProjectileGenome genomeCopy = new WeaponProjectileGenome(templateGenome.weaponProjectileList[i]);
            weaponProjectileList.Add(genomeCopy);
        }
        weaponTazerList = new List<WeaponTazerGenome>();
        for (int i = 0; i < templateGenome.weaponTazerList.Count; i++) {
            WeaponTazerGenome genomeCopy = new WeaponTazerGenome(templateGenome.weaponTazerList[i]);
            weaponTazerList.Add(genomeCopy);
        }
    }

    public void InitializeRandomBrainGenome(float initialWeightMultiplier) {
        brainGenome = new BrainGenome();

        for (int i = 0; i < basicWheelList.Count; i++) {
            basicWheelList[i].InitializeBrainGenome(brainGenome.neuronList);
        }
        for (int i = 0; i < basicJointList.Count; i++) {
            basicJointList[i].InitializeBrainGenome(brainGenome.neuronList);
        }
        for (int i = 0; i < contactSensorList.Count; i++) {
            contactSensorList[i].InitializeBrainGenome(brainGenome.neuronList);
        }
        for (int i = 0; i < healthModuleList.Count; i++) {
            healthModuleList[i].InitializeBrainGenome(brainGenome.neuronList);
        }
        for (int i = 0; i < oscillatorInputList.Count; i++) {
            oscillatorInputList[i].InitializeBrainGenome(brainGenome.neuronList);
        }
        for (int i = 0; i < raycastSensorList.Count; i++) {
            raycastSensorList[i].InitializeBrainGenome(brainGenome.neuronList);
        }
        for (int i = 0; i < targetSensorList.Count; i++) {
            targetSensorList[i].InitializeBrainGenome(brainGenome.neuronList);            
        }        
        for (int i = 0; i < thrusterList.Count; i++) {
            thrusterList[i].InitializeBrainGenome(brainGenome.neuronList);            
        }
        for (int i = 0; i < torqueList.Count; i++) {
            torqueList[i].InitializeBrainGenome(brainGenome.neuronList);            
        }
        for (int i = 0; i < valueInputList.Count; i++) {
            valueInputList[i].InitializeBrainGenome(brainGenome.neuronList);            
        }        
        for (int i = 0; i < weaponProjectileList.Count; i++) {
            weaponProjectileList[i].InitializeBrainGenome(brainGenome.neuronList);            
        }
        for (int i = 0; i < weaponTazerList.Count; i++) {
            weaponTazerList[i].InitializeBrainGenome(brainGenome.neuronList);            
        }

        int numInputs = 0;
        for(int i = 0; i < brainGenome.neuronList.Count; i++) {
            if(brainGenome.neuronList[i].neuronType == NeuronGenome.NeuronType.In) {
                numInputs++;
            }
        }
        // !!!!! ===== UPGRADE NOTE: !!!!!!!!!!!!
        //  Add support for initial traditional hidden layers - generalize down to 0 to support all initializations
        // !!!!! ===== UPGRADE NOTE: !!!!!!!!!!!!

        // Create Hidden nodes TEMP!!!!
        for (int i = 0; i < numInputs; i++) {
            //NeuronGenome neuron = new NeuronGenome(NeuronGenome.NeuronType.Hid, 20, i);
            //brainGenome.neuronList.Add(neuron);
        }

        // Create initial connections -- :
        List<NeuronGenome> inputNeuronList = new List<NeuronGenome>();
        List<NeuronGenome> hiddenNeuronList = new List<NeuronGenome>();
        List<NeuronGenome> outputNeuronList = new List<NeuronGenome>();
        for (int i = 0; i < brainGenome.neuronList.Count; i++) {
            if (brainGenome.neuronList[i].neuronType == NeuronGenome.NeuronType.In) {
                inputNeuronList.Add(brainGenome.neuronList[i]);
            }
            if (brainGenome.neuronList[i].neuronType == NeuronGenome.NeuronType.Hid) {
                hiddenNeuronList.Add(brainGenome.neuronList[i]);
            }
            if (brainGenome.neuronList[i].neuronType == NeuronGenome.NeuronType.Out) {
                outputNeuronList.Add(brainGenome.neuronList[i]);
            }
        }
        // Initialize fully connected with all weights Random
        for (int i = 0; i < outputNeuronList.Count; i++) {
            for (int j = 0; j < inputNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * initialWeightMultiplier;
                LinkGenome linkGenome = new LinkGenome(inputNeuronList[j].nid.moduleID, inputNeuronList[j].nid.neuronID, outputNeuronList[i].nid.moduleID, outputNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }
        /*for (int i = 0; i < outputNeuronList.Count; i++) {
            for(int j = 0; j < hiddenNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * initialWeightMultiplier;
                LinkGenome linkGenome = new LinkGenome(hiddenNeuronList[j].nid.moduleID, hiddenNeuronList[j].nid.neuronID, outputNeuronList[i].nid.moduleID, outputNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }
        for (int i = 0; i < hiddenNeuronList.Count; i++) {
            for (int j = 0; j < inputNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * initialWeightMultiplier;
                LinkGenome linkGenome = new LinkGenome(inputNeuronList[j].nid.moduleID, inputNeuronList[j].nid.neuronID, hiddenNeuronList[i].nid.moduleID, hiddenNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }*/

        //PrintBrainGenome();
    }

    public void PrintBrainGenome() {
        string neuronText = "";
        for (int i = 0; i < brainGenome.neuronList.Count; i++) {
            neuronText += "(" + brainGenome.neuronList[i].nid.moduleID.ToString() + "," + brainGenome.neuronList[i].nid.neuronID.ToString() + ")\n";
        }
        Debug.Log(neuronText);
        string linkText = "";
        for (int i = 0; i < brainGenome.linkList.Count; i++) {
            linkText += "(" + brainGenome.linkList[i].fromModuleID.ToString() + "," + brainGenome.linkList[i].fromNeuronID.ToString() + ") ==> (" +
                        brainGenome.linkList[i].toModuleID.ToString() + "," + brainGenome.linkList[i].toNeuronID.ToString() + ")\n";
        }
        Debug.Log(linkText);
    }
}

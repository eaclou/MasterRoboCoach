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

    public BrainGenome brainGenome;
    public List<SegmentGenome> segmentList;
    // Modules:
    public List<TargetSensorGenome> targetSensorList;
    public List<RaycastSensorGenome> raycastSensorList;
    public List<ThrusterGenome> thrusterList;
    public List<TorqueGenome> torqueList;
    public List<ValueInputGenome> valueInputList;

    // Constructor
    public AgentGenome(int index) {
        this.index = index;
    }

    /*[MenuItem("Assets/Create/My Scriptable Object")]
    public static void CreateMyAsset() {
        AgentGenome asset = ScriptableObject.CreateInstance<AgentGenome>();

        AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }*/
    public void CopyGenomeFromTemplate(AgentGenome templateGenome) {
        // This method creates a clone of the provided ScriptableObject Genome - should have no shared references!!!
        // copy segment list:
        segmentList = new List<SegmentGenome>();
        for(int i = 0; i < templateGenome.segmentList.Count; i++) {
            SegmentGenome segmentCopy = new SegmentGenome(templateGenome.segmentList[i]);
            segmentList.Add(segmentCopy);
        }      
        // copy module lists:
        targetSensorList = new List<TargetSensorGenome>();
        for(int i = 0; i < templateGenome.targetSensorList.Count; i++) {
            TargetSensorGenome genomeCopy = new TargetSensorGenome(templateGenome.targetSensorList[i]);
            targetSensorList.Add(genomeCopy);
        }        
        raycastSensorList = new List<RaycastSensorGenome>();
        for(int i = 0; i < templateGenome.raycastSensorList.Count; i++) {
            RaycastSensorGenome genomeCopy = new RaycastSensorGenome(templateGenome.raycastSensorList[i]);
            raycastSensorList.Add(genomeCopy);
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

        // For now this is fine -- but eventually might want to copy brainGenome from saved asset!
        brainGenome = new BrainGenome();  // creates neuron and axonLists
        InitializeRandomBrainGenome();
    }

    public void TempInitializeTestGenome() {
        SegmentGenome rootSegment = new SegmentGenome(-1);
        
        TargetSensorGenome targetSensor = new TargetSensorGenome(0, 1);
        RaycastSensorGenome raycastSensor = new RaycastSensorGenome(0, 2);
        ThrusterGenome thruster = new ThrusterGenome(0, 3);
        TorqueGenome torque = new TorqueGenome(0, 4);
        ValueInputGenome valueInput = new ValueInputGenome(0, 5);

        segmentList = new List<SegmentGenome>();
        segmentList.Add(rootSegment);

        targetSensorList = new List<TargetSensorGenome>();
        targetSensorList.Add(targetSensor);
        raycastSensorList = new List<RaycastSensorGenome>();
        raycastSensorList.Add(raycastSensor);
        thrusterList = new List<ThrusterGenome>();
        thrusterList.Add(thruster);
        torqueList = new List<TorqueGenome>();
        torqueList.Add(torque);
        valueInputList = new List<ValueInputGenome>();
        valueInputList.Add(valueInput);

        brainGenome = new BrainGenome();
        InitializeRandomBrainGenome();
        //PrintBrainGenome();
    }

    public void InitializeRandomBrainGenome() {
        //Debug.Log("InitializeRandomBrain");
        // Create Initial Neurons:
        for (int i = 0; i < targetSensorList.Count; i++) {
            NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 0);
            NeuronGenome neuronY = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 1);
            NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 2);
            NeuronGenome neuronVel = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 3);
            NeuronGenome neuronAngVel = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 4);
            NeuronGenome neuronInTarget = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 5);
            brainGenome.neuronList.Add(neuronX);
            brainGenome.neuronList.Add(neuronY);
            brainGenome.neuronList.Add(neuronZ);
            brainGenome.neuronList.Add(neuronVel);
            brainGenome.neuronList.Add(neuronAngVel);
            brainGenome.neuronList.Add(neuronInTarget);
        }
        for (int i = 0; i < raycastSensorList.Count; i++) {
            //Debug.Log("raycastSensorList AgentGenome Init");
            NeuronGenome neuronLeft = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 0);
            NeuronGenome neuronLeftCenter = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 1);
            NeuronGenome neuronCenter = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 2);
            NeuronGenome neuronRightCenter = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 3);
            NeuronGenome neuronRight = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 4);
            NeuronGenome neuronBack = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 5);
            NeuronGenome neuronCenterShort = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 6);
            brainGenome.neuronList.Add(neuronLeft);
            brainGenome.neuronList.Add(neuronLeftCenter);
            brainGenome.neuronList.Add(neuronCenter);
            brainGenome.neuronList.Add(neuronRightCenter);
            brainGenome.neuronList.Add(neuronRight);
            brainGenome.neuronList.Add(neuronBack);
            brainGenome.neuronList.Add(neuronCenterShort);
        }
        for (int i = 0; i < thrusterList.Count; i++) {
            NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.Out, thrusterList[i].inno, 0);
            NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.Out, thrusterList[i].inno, 1);
            brainGenome.neuronList.Add(neuronZ);
            brainGenome.neuronList.Add(neuronX);
        }
        for (int i = 0; i < torqueList.Count; i++) {
            NeuronGenome neuron = new NeuronGenome(NeuronGenome.NeuronType.Out, torqueList[i].inno, 0);
            brainGenome.neuronList.Add(neuron);
        }
        for (int i = 0; i < valueInputList.Count; i++) {
            NeuronGenome neuron = new NeuronGenome(NeuronGenome.NeuronType.In, valueInputList[i].inno, 0);
            brainGenome.neuronList.Add(neuron);
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
            //NeuronGenome neuron = new NeuronGenome(NeuronGenome.NeuronType.Hid, 6, i);
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
                float randomWeight = Gaussian.GetRandomGaussian() * 1f;
                LinkGenome linkGenome = new LinkGenome(inputNeuronList[j].nid.moduleID, inputNeuronList[j].nid.neuronID, outputNeuronList[i].nid.moduleID, outputNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }
        /*for (int i = 0; i < outputNeuronList.Count; i++) {
            for(int j = 0; j < hiddenNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * 0.1f;
                LinkGenome linkGenome = new LinkGenome(hiddenNeuronList[j].nid.moduleID, hiddenNeuronList[j].nid.neuronID, outputNeuronList[i].nid.moduleID, outputNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }
        for (int i = 0; i < hiddenNeuronList.Count; i++) {
            for (int j = 0; j < inputNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * 0.1f;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGenome {

    // Naive Implementation!!! Re-factor later when I have a better idea of how to structure this!!
    // Agent Genome:
        // Brain Genome
        // Module Lists - one per module type - points to parent GameObjectID
        // Segment List -- nodeList


    public BrainGenome brainGenome;
    public List<SegmentGenome> segmentList;
    // Modules:
    public List<TargetSensorGenome> targetSensorList;
    public List<RaycastSensorGenome> raycastSensorList;
    public List<ThrusterGenome> thrusterList;
    public List<TorqueGenome> torqueList;
    public List<ValueInputGenome> valueInputList;
	
    // Constructor
    public AgentGenome() {

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
            brainGenome.neuronList.Add(neuronX);
            brainGenome.neuronList.Add(neuronY);
            brainGenome.neuronList.Add(neuronZ);
            brainGenome.neuronList.Add(neuronVel);
            brainGenome.neuronList.Add(neuronAngVel);
        }
        for (int i = 0; i < raycastSensorList.Count; i++) {
            //Debug.Log("raycastSensorList AgentGenome Init");
            NeuronGenome neuronLeft = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 0);
            NeuronGenome neuronLeftCenter = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 1);
            NeuronGenome neuronCenter = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 2);
            NeuronGenome neuronRightCenter = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 3);
            NeuronGenome neuronRight = new NeuronGenome(NeuronGenome.NeuronType.In, raycastSensorList[i].inno, 4);
            brainGenome.neuronList.Add(neuronLeft);
            brainGenome.neuronList.Add(neuronLeftCenter);
            brainGenome.neuronList.Add(neuronCenter);
            brainGenome.neuronList.Add(neuronRightCenter);
            brainGenome.neuronList.Add(neuronRight);
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
        // Create Hidden nodes TEMP!!!!
        for(int i = 0; i < numInputs; i++) {
            NeuronGenome neuron = new NeuronGenome(NeuronGenome.NeuronType.Hid, 6, i);
            brainGenome.neuronList.Add(neuron);
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
        /*for (int i = 0; i < outputNeuronList.Count; i++) {
            for (int j = 0; j < inputNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * 0.5f;
                LinkGenome linkGenome = new LinkGenome(inputNeuronList[j].nid.moduleID, inputNeuronList[j].nid.neuronID, outputNeuronList[i].nid.moduleID, outputNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }*/
        for (int i = 0; i < outputNeuronList.Count; i++) {
            for(int j = 0; j < hiddenNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * 0.5f;
                LinkGenome linkGenome = new LinkGenome(hiddenNeuronList[j].nid.moduleID, hiddenNeuronList[j].nid.neuronID, outputNeuronList[i].nid.moduleID, outputNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }
        for (int i = 0; i < hiddenNeuronList.Count; i++) {
            for (int j = 0; j < inputNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * 0.5f;
                LinkGenome linkGenome = new LinkGenome(inputNeuronList[j].nid.moduleID, inputNeuronList[j].nid.neuronID, hiddenNeuronList[i].nid.moduleID, hiddenNeuronList[i].nid.neuronID, randomWeight, true);
                brainGenome.linkList.Add(linkGenome);
            }
        }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain {
    public List<Neuron> neuronList;
    public Dictionary<NID,int> IDs;
    public List<Axon> axonList;

    

	public Brain(BrainGenome genome, Agent agent) {
        RebuildBrain(genome, agent);
    }

    public void RebuildBrain(BrainGenome genome, Agent agent) {
        // Create Neurons:
        neuronList = new List<Neuron>();
        IDs = new Dictionary<NID, int>();
        // I need access to all of the Modules in order to link the neuron values to the module values!!!!
        for (int i = 0; i < genome.neuronList.Count; i++) {
            Neuron neuron = new Neuron();
            // Find out what the source of this Neuron is -- i.e. which Module???
            //Debug.Log("NeuronID: (" + genome.neuronList[i].nid.moduleID.ToString() + "," + genome.neuronList[i].nid.neuronID.ToString() + ")");
            if(genome.neuronList[i].neuronType != NeuronGenome.NeuronType.Hid) {
                agent.MapNeuronToModule(genome.neuronList[i].nid, neuron);
            }
            else {
                neuron.neuronType = NeuronGenome.NeuronType.Hid;
                neuron.currentValue = new float[1];
            }            
            IDs.Add(genome.neuronList[i].nid, i);
            neuronList.Add(neuron);
        }

        // Create Axons:
        axonList = new List<Axon>();
        for (int i = 0; i < genome.linkList.Count; i++) {
            // find out neuronIDs:
            int fromID = -1;
            IDs.TryGetValue(new NID(genome.linkList[i].fromModuleID, genome.linkList[i].fromNeuronID), out fromID);
            int toID = IDs[new NID(genome.linkList[i].toModuleID, genome.linkList[i].toNeuronID)];

            //Debug.Log(fromID.ToString() + " --> " + toID.ToString() + ", " + genome.linkList[i].weight.ToString());
            //int fromID = IDs < new NID(genome.linkList[i].fromModuleID, genome.linkList[i].fromNeuronID) >;
            Axon axon = new Axon(fromID, toID, genome.linkList[i].weight);
            axonList.Add(axon);
        }
    }

    public void BrainMasterFunction() {
        // ticks state of brain forward 1 timestep

        // NAIVE APPROACH:
        // run through all links and save sum values in target neurons
        for(int i = 0; i < axonList.Count; i++) {
            // Find input neuron and multiply its value by the axon weight --- add that to output neuron total:
            neuronList[axonList[i].toID].inputTotal += axonList[i].weight * neuronList[axonList[i].fromID].currentValue[0];
        }
        // Once all axons are calculated, process the neurons:
        for(int j = 0; j < neuronList.Count; j++) {
            neuronList[j].previousValue = neuronList[j].currentValue[0]; // Save previous state
            if(neuronList[j].neuronType != NeuronGenome.NeuronType.In) {
                neuronList[j].currentValue[0] = TransferFunctions.Evaluate(TransferFunctions.TransferFunction.RationalSigmoid, neuronList[j].inputTotal);
            }
            // now zero out inputSum:
            neuronList[j].inputTotal = 0f;
        }
    }

    public void PrintBrain() {
        string neuronText = "";
        for(int i = 0; i < neuronList.Count; i++) {
            neuronText += "Neuron " + i.ToString() + ": " + neuronList[i].currentValue[0].ToString() + "\n";
        }        
        string axonText = "";
        for(int j = 0; j < axonList.Count; j++) {
            axonText += "Axon " + j.ToString() + ": (" + axonList[j].fromID.ToString() + "," + axonList[j].toID.ToString() + ") " + axonList[j].weight.ToString() + "\n"; 
        }
        Debug.Log(neuronText + "\n" + axonText);
    }
}

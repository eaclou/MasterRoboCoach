using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrainGenome {

    public List<NeuronGenome> bodyNeuronList;
    public List<NeuronGenome> hiddenNeuronList;
    public List<LinkGenome> linkList;
	
    public BrainGenome() {
        
    }

    public void InitializeNewBrainGenomeLists() {
        bodyNeuronList = new List<NeuronGenome>();
        hiddenNeuronList = new List<NeuronGenome>();
        linkList = new List<LinkGenome>();
    }

    public void SetBodyNeuronsFromTemplate(BodyGenome templateBody) {
        if(bodyNeuronList == null) {
            bodyNeuronList = new List<NeuronGenome>();
        }
        else {
            bodyNeuronList.Clear();
        }

        InitializeBodyNeurons(templateBody);        
    }

    public void InitializeRandomBrainGenome(BodyGenome bodyGenome, float initialWeightMultiplier) {
        InitializeNewBrainGenomeLists();
        InitializeBodyNeurons(bodyGenome);
        InitializeAxons(initialWeightMultiplier);
    }

    public void InitializeBodyNeurons(BodyGenome bodyGenome) {
        for (int i = 0; i < bodyGenome.basicWheelList.Count; i++) {
            bodyGenome.basicWheelList[i].InitializeBrainGenome(bodyNeuronList); // Creates Neurons based on this Module and adds them to provided List
        }
        for (int i = 0; i < bodyGenome.basicJointList.Count; i++) {
            bodyGenome.basicJointList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.contactSensorList.Count; i++) {
            bodyGenome.contactSensorList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.healthModuleList.Count; i++) {
            bodyGenome.healthModuleList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.oscillatorInputList.Count; i++) {
            bodyGenome.oscillatorInputList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.raycastSensorList.Count; i++) {
            bodyGenome.raycastSensorList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.targetSensorList.Count; i++) {
            bodyGenome.targetSensorList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.thrusterList.Count; i++) {
            bodyGenome.thrusterList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.torqueList.Count; i++) {
            bodyGenome.torqueList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.valueInputList.Count; i++) {
            bodyGenome.valueInputList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.weaponProjectileList.Count; i++) {
            bodyGenome.weaponProjectileList[i].InitializeBrainGenome(bodyNeuronList);
        }
        for (int i = 0; i < bodyGenome.weaponTazerList.Count; i++) {
            bodyGenome.weaponTazerList[i].InitializeBrainGenome(bodyNeuronList);
        }
    }

    public void InitializeAxons(float initialWeightMultiplier) {
        int numInputs = 0;
        for (int i = 0; i < bodyNeuronList.Count; i++) {
            if (bodyNeuronList[i].neuronType == NeuronGenome.NeuronType.In) {
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
        //List<NeuronGenome> hiddenNeuronList = new List<NeuronGenome>();
        List<NeuronGenome> outputNeuronList = new List<NeuronGenome>();
        for (int i = 0; i < bodyNeuronList.Count; i++) {
            if (bodyNeuronList[i].neuronType == NeuronGenome.NeuronType.In) {
                inputNeuronList.Add(bodyNeuronList[i]);
            }
            //if (brainGenome.neuronList[i].neuronType == NeuronGenome.NeuronType.Hid) {
            //    hiddenNeuronList.Add(brainGenome.neuronList[i]);
            //}
            if (bodyNeuronList[i].neuronType == NeuronGenome.NeuronType.Out) {
                outputNeuronList.Add(bodyNeuronList[i]);
            }
        }
        // Initialize fully connected with all weights Random
        for (int i = 0; i < outputNeuronList.Count; i++) {
            for (int j = 0; j < inputNeuronList.Count; j++) {
                float randomWeight = Gaussian.GetRandomGaussian() * initialWeightMultiplier;
                LinkGenome linkGenome = new LinkGenome(inputNeuronList[j].nid.moduleID, inputNeuronList[j].nid.neuronID, outputNeuronList[i].nid.moduleID, outputNeuronList[i].nid.neuronID, randomWeight, true);
                linkList.Add(linkGenome);
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

    public void SetToMutatedCopyOfParentGenome(BrainGenome parentGenome, float mutationChance, float mutationStepSize) {
        this.bodyNeuronList = parentGenome.bodyNeuronList; // UNSUSTAINABLE!!! might work now since all neuronLists are identical
        // Alternate: SetBodyNeuronsFromTemplate(BodyGenome templateBody);

        hiddenNeuronList = new List<NeuronGenome>();

        linkList = new List<LinkGenome>();
        for (int i = 0; i < parentGenome.linkList.Count; i++) {
            LinkGenome newLinkGenome = new LinkGenome(parentGenome.linkList[i].fromModuleID, parentGenome.linkList[i].fromNeuronID, parentGenome.linkList[i].toModuleID, parentGenome.linkList[i].toNeuronID, parentGenome.linkList[i].weight, true);
            float rand = UnityEngine.Random.Range(0f, 1f);
            if (rand < mutationChance) {
                float randomWeight = Gaussian.GetRandomGaussian();
                newLinkGenome.weight = Mathf.Lerp(newLinkGenome.weight, randomWeight, mutationStepSize);
            }
            linkList.Add(newLinkGenome);
        }        
    }

    /*public void MutateRandomly(float mutationChance) {
        for(int i = 0; i < linkList.Count; i++) {
            float rand = UnityEngine.Random.Range(0f, 1f);
            if(rand < mutationChance) {                
                float randomWeight = Gaussian.GetRandomGaussian();
                //Debug.Log("mutation! old: " + linkList[i].weight.ToString() + ", new: " + randomWeight.ToString());
                LinkGenome mutatedLink = new LinkGenome(linkList[i].fromModuleID, linkList[i].fromNeuronID, linkList[i].toModuleID, linkList[i].toNeuronID, randomWeight, true);
                linkList[i] = mutatedLink;
            }            
        }
    }*/
}

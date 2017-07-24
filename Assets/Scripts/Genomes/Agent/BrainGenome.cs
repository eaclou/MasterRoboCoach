using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrainGenome {

    public List<NeuronGenome> neuronList;
    public List<LinkGenome> linkList;
	
    public BrainGenome() {
        neuronList = new List<NeuronGenome>();
        linkList = new List<LinkGenome>();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThrusterEffector : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public float[] throttleX;
    public float[] throttleY;
    public float[] throttleZ;    

    public float horsepowerX;
    public float horsepowerY;
    public float horsepowerZ;

    public bool useX;
    public bool useY;
    public bool useZ;

    public GameObject parentBody;
    public Vector3 forcePoint;

    //public ParticleSystem rearThrusterParticle;
    //public Material rearThrusterMat;

    public ThrusterComponent thrusterComponent;

    //public enum 

	public ThrusterEffector() {
        
    }

    public void Initialize(ThrusterGenome genome, Agent agent) {
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;

        forcePoint = genome.forcePoint;
        throttleX = new float[1];
        throttleY = new float[1];
        throttleZ = new float[1];        

        horsepowerX = genome.horsepowerX;
        horsepowerY = genome.horsepowerY;
        horsepowerZ = genome.horsepowerZ;

        useX = genome.useX;
        useY = genome.useY;
        useZ = genome.useZ;

        parentBody = agent.segmentList[parentID];

        // Create renderable elements:        
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            //Debug.Log("neuron match!!! thrusterEffector");
            if (nid.neuronID == 0) {
                neuron.currentValue = throttleX;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = throttleY;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 2) {
                neuron.currentValue = throttleZ;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
        }
    }

    public void Tick() {
        
        parentBody.GetComponent<Rigidbody>().AddForceAtPosition(parentBody.transform.right * Mathf.Clamp01(throttleX[0]) * horsepowerX, parentBody.GetComponent<Rigidbody>().worldCenterOfMass + forcePoint);
        parentBody.GetComponent<Rigidbody>().AddForceAtPosition(parentBody.transform.up * Mathf.Clamp01(throttleY[0]) * horsepowerY, parentBody.GetComponent<Rigidbody>().worldCenterOfMass + forcePoint);
        parentBody.GetComponent<Rigidbody>().AddForceAtPosition(parentBody.transform.forward * Mathf.Clamp01(throttleZ[0]) * horsepowerZ, parentBody.GetComponent<Rigidbody>().worldCenterOfMass + forcePoint); //.AddRelativeForce(new Vector3(0f, 0f, Mathf.Clamp01(throttle[0])) * horsepowerZ, ForceMode.Force);

        if (isVisible) {
            thrusterComponent.throttle = throttleY[0];
            thrusterComponent.Tick();

            /*ParticleSystem.MainModule mainModule = rearThrusterParticle.main;
            ParticleSystem.MinMaxGradient col = mainModule.startColor;
            col.color = new Color(col.color.r, col.color.g, col.color.b, Mathf.Clamp01(throttle[0]));
            mainModule.startColor = col;
            rearThrusterMat.SetColor("_EmissionColor", new Color(0.66f, 0.93f, 1f) * 12f * Mathf.Clamp01(throttle[0]));
            */
        }        
    }
}

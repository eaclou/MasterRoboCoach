using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponTazer : AgentModuleBase {

    //public int parentID;
    //public int inno;
    public float[] throttle;
    public float[] energy;
    public float[] damageInflicted;

    //public ParticleSystem particles;
    public WeaponTazerComponent weaponTazerComponent;

    public GameObject parentBody;
    public Vector3 muzzleLocation;

    public WeaponTazer() {
        /*parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        energy = new float[1];
        damageInflicted = new float[1];*/
    }

    public void Initialize(WeaponTazerGenome genome, Agent agent) {
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;

        muzzleLocation = genome.muzzleLocation;
        throttle = new float[1];
        energy = new float[1];
        damageInflicted = new float[1];

        parentBody = agent.segmentList[parentID];
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = energy;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = throttle;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
        }
    }

    public void Tick() {
        float rayMaxDistance = 5f;
        damageInflicted[0] = 0f;
                
        bool isFiring = false;
        if (throttle[0] > 0f) {
            if (energy[0] > 0.1f) {
                energy[0] -= 0.1f; // costs energy to fire

                Vector3 rayOrigin = parentBody.transform.position + muzzleLocation;
                Vector3 rayDir = parentBody.transform.TransformDirection(new Vector3(0f, 0f, 1f));
                RaycastHit hit;
                if (Physics.Raycast(rayOrigin, rayDir, out hit, rayMaxDistance)) {
                    if (hit.collider.GetComponent<HealthModuleComponent>() != null) {
                        hit.collider.GetComponent<HealthModuleComponent>().InflictDamage(25f);
                        damageInflicted[0] = 25f;
                    }
                }
                isFiring = true;
                
            }
        }
        else {
            energy[0] += 0.01f;
        }

        if (isVisible) {
            if (isFiring) {
                weaponTazerComponent.throttle = 1f;
            }
            else {
                weaponTazerComponent.throttle = 0f;
            }
            weaponTazerComponent.Tick();
        }
    }


    /*public string GetParticleSystemURL() {
        return "Prefabs/ParticleSystems/WeaponTazer";
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponProjectile : AgentModuleBase {

    //public int parentID;
    //public int inno;
    public float[] throttle;
    public float[] energy;
    public float[] damageInflicted;

    public GameObject parentBody;
    public Vector3 muzzleLocation;

    public WeaponProjectileComponent weaponProjectileComponent;
    //public ParticleSystem particles;

    public WeaponProjectile() {
        /*parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        energy = new float[1];
        damageInflicted = new float[1];*/
    }

    public void Initialize(WeaponProjectileGenome genome, Agent agent) {
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
        float rayMaxDistance = 30f;
        damageInflicted[0] = 0f;
        
        bool isFiring = false;
        if (throttle[0] > 0f) {
            if (energy[0] > 0.01f) {
                energy[0] -= 0.01f; // costs energy to fire

                Vector3 rayOrigin = parentBody.transform.position + muzzleLocation;
                Vector3 rayDir = parentBody.transform.TransformDirection(new Vector3(0f, 0f, 1f));
                RaycastHit hit;
                if (Physics.Raycast(rayOrigin, rayDir, out hit, rayMaxDistance)) {
                    if (hit.collider.GetComponent<HealthModuleComponent>() != null) {
                        hit.collider.GetComponent<HealthModuleComponent>().InflictDamage(5f);
                        damageInflicted[0] = 5f;                          
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
                weaponProjectileComponent.throttle = 1f;                
            }
            else {
                weaponProjectileComponent.throttle = 0f;
            }
            weaponProjectileComponent.Tick();
        }
    }

    /*public string GetParticleSystemURL() {
        return "Prefabs/ParticleSystems/WeaponProjectile";
    }*/
}

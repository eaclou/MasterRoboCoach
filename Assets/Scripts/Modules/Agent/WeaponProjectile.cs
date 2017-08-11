using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponProjectile {

    public int parentID;
    public int inno;
    public float[] throttle;
    public float[] energy;
    public float[] damageInflicted;

    public GameObject parentBody;
    public Vector3 muzzleLocation;
    public ParticleSystem particles;

    public WeaponProjectile(WeaponProjectileGenome genome) {
        /*parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        energy = new float[1];
        damageInflicted = new float[1];*/
    }

    public void Initialize(WeaponProjectileGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        energy = new float[1];
        damageInflicted = new float[1];
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

    public void Tick(bool isVisible) {
        float rayMaxDistance = 30f;
        damageInflicted[0] = 0f;

        if (isVisible) {
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = false;
        }

        if (throttle[0] > 0f) {
            if (energy[0] > 0.01f) {
                energy[0] -= 0.01f; // costs energy to fire

                Vector3 rayOrigin = parentBody.transform.position + muzzleLocation;
                Vector3 rayDir = parentBody.transform.TransformDirection(new Vector3(0f, 0f, 1f));
                RaycastHit hit;
                //debugProjectileEnd = rayOrigin + center.normalized * rayMaxDistance;
                if (Physics.Raycast(rayOrigin, rayDir, out hit, rayMaxDistance)) {
                    if (hit.collider.GetComponent<HealthModuleComponent>() != null) {
                        hit.collider.GetComponent<HealthModuleComponent>().TakeDamage(5f);
                        damageInflicted[0] = 5f;
                        //debugProjectileEnd = rayOrigin + center.normalized * hit.distance;                            
                    }
                }
                else {

                }
                //debugProjectileOn = true;
                //debugProjectileSource = rayOrigin;
                //debugProjectileEnd = rayOrigin + center.normalized * rayMaxDistance;

                if (isVisible) {
                    ParticleSystem.EmissionModule emission = particles.emission;
                    emission.enabled = true;
                }
            }
        }
        else {
            energy[0] += 0.01f;
        }
    }

    public string GetParticleSystemURL() {
        return "Prefabs/ParticleSystems/WeaponProjectile";
    }
}

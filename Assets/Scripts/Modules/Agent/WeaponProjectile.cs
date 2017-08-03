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

    public ParticleSystem particles;

    public WeaponProjectile(WeaponProjectileGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        energy = new float[1];
        damageInflicted = new float[1];
    }

    public string GetParticleSystemURL() {
        return "ParticleSystems/WeaponProjectile";
    }
}

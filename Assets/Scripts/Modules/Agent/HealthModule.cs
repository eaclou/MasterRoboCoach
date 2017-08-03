using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthModule {
    public int parentID;
    public int inno;
    public float maxHealth = 200f;
    public float prevHealth;
    public float health;
    public float[] healthSensor;
    public float[] takingDamage;

    public HealthModuleComponent component;

    public HealthModule(HealthGenome genome) {
        healthSensor = new float[1];
        takingDamage = new float[1];
        health = maxHealth;
        prevHealth = health;
        parentID = genome.parentID;
        inno = genome.inno;
    }
}

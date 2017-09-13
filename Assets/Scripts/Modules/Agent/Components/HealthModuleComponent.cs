using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModuleComponent : MonoBehaviour {
    
    //public float maxHealth = 200f;
    //public float prevHealth;
    //public float health;    

    public HealthModule healthModule;

	// Use this for initialization
	void Awake () {
        //InitializeModule();
    }
    
	public void InflictDamage(float damage) {
        healthModule.InflictDamage(damage);
        //healthModule.health -= damage;
        //if (healthModule.health < 0f)
        //    healthModule.health = 0f;
    }
}

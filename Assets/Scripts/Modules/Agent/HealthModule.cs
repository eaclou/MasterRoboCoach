using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModule : MonoBehaviour {

    public float maxHealth = 100f;
    public float health;

	// Use this for initialization
	void Start () {
        health = maxHealth;
	}
	
	public void TakeDamage(float damage) {
        health -= damage;
        if (health < 0f)
            health = 0f;
    }
}

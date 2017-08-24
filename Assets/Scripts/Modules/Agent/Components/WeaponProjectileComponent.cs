using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileComponent : MonoBehaviour {

    public float throttle;

    public ParticleSystem muzzleFlashParticle;
    public Light muzzleLight;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Tick() {        

        ParticleSystem.EmissionModule emission = muzzleFlashParticle.emission;
        if(throttle == 1f) {
            emission.enabled = true;
            muzzleLight.gameObject.SetActive(true);
        }
        else {
            emission.enabled = false;
            muzzleLight.gameObject.SetActive(false);
        }
    }
}

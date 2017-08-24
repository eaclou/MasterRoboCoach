using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterComponent : MonoBehaviour {

    public float throttle;

    public ParticleSystem rearThrusterParticle;
    public Material rearThrusterMat;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Tick() {
        // Update all visible elements based on throttle value:
        ParticleSystem.MainModule mainModule = rearThrusterParticle.main;
        ParticleSystem.MinMaxGradient col = mainModule.startColor;
        col.color = new Color(col.color.r, col.color.g, col.color.b, Mathf.Clamp01(throttle));
        mainModule.startColor = col;
        rearThrusterMat.SetColor("_EmissionColor", new Color(0.66f, 0.93f, 1f) * 12f * Mathf.Clamp01(throttle));            
    }
}

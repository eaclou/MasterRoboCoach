using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHitEffects : MonoBehaviour {

    public bool active = false;

    public ParticleSystem partCustomHeight;    
    public ParticleSystem.EmitParams emitterParamsCustomHeight;
    private float maxForceValueCustomHeight = 4f;
    private float maxSizeCustomHeight = 0.11f;

    public ParticleSystem partImpactDust;
    public ParticleSystem.EmitParams emitterParamsImpactDust;
    private float maxForceValueImpactDust = 4f;
    private float maxSizeImpactDust = 0.5f;

    public ParticleSystem partImpactPebbles;
    public ParticleSystem.EmitParams emitterParamsImpactPebbles;
    private float maxForceValueImpactPebbles = 4f;
    private float maxSizeImpactPebbles = 0.11f;

    // Use this for initialization
    void Start () {
        

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void SpawnParticles(Collision collision) {
        float force = collision.impulse.magnitude;
        float velMag = collision.relativeVelocity.magnitude;

        float val = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(force / maxForceValueCustomHeight));
        emitterParamsCustomHeight.startColor = new Color(val, val, val);
        float size = Mathf.Lerp(maxSizeCustomHeight * 0.7f, maxSizeCustomHeight, Mathf.Clamp01(force / maxForceValueCustomHeight));
        emitterParamsCustomHeight.startSize = size;
        emitterParamsCustomHeight.position = collision.contacts[0].point;
        partCustomHeight.Emit(emitterParamsCustomHeight, 1);

        //emitterParamsImpactDust.startSize = maxSizeImpactDust;
        float dustForceThreshold = 3f;
        float dustVelThreshold = 1f;
        if(force > dustForceThreshold || velMag > dustVelThreshold) {
            emitterParamsImpactDust.position = collision.contacts[0].point;
            emitterParamsImpactDust.velocity = collision.relativeVelocity * UnityEngine.Random.Range(0.25f, 1f);
            partImpactDust.Emit(emitterParamsImpactDust, 1);

            //emitterParamsImpactPebbles.startSize = maxSizeImpactPebbles;
            emitterParamsImpactPebbles.position = collision.contacts[0].point;
            emitterParamsImpactPebbles.velocity = collision.relativeVelocity * UnityEngine.Random.Range(0.1f, 1.5f) + UnityEngine.Random.insideUnitSphere * velMag * 0.35f;
            partImpactPebbles.Emit(emitterParamsImpactPebbles, 4);
        }
        

        
    }

    private void OnCollisionEnter(Collision collision) {
        //Debug.Log(active.ToString() + ", CollisionHitEffects enter: " + collision.contacts[0].point.ToString());
        
        if (active) {
            SpawnParticles(collision);
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (active) {
            SpawnParticles(collision);
        }
    }

    private void OnCollisionExit(Collision collision) {

    }
}

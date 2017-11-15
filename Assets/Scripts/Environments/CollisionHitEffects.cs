using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHitEffects : MonoBehaviour {

    public ParticleSystem part;
    public bool active = false;
    public ParticleSystem.EmitParams emitterParamsDefault;

    private float maxForceValue = 4f;
    private float maxSize = 0.11f;

    // Use this for initialization
    void Start () {
        

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnCollisionEnter(Collision collision) {
        //Debug.Log(active.ToString() + ", CollisionHitEffects enter: " + collision.contacts[0].point.ToString());
        
        if (active) {
            //collision.relativeVelocity
            //particleSystem = Resources.Load("Prefabs/ParticleSystems/GroundImpactDustA", typeof(GameObject)) as GameObject;
            //GameObject go = Instantiate(particleSystem, collision.contacts[0].point, Quaternion.Euler(-90f, 0f, 0f)) as GameObject;
            //Destroy(go, 0.25f);
            float force = collision.impulse.magnitude;
            float val = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(force / maxForceValue));
            emitterParamsDefault.startColor = new Color(val, val, val);
            float size = Mathf.Lerp(maxSize * 0.7f, maxSize, Mathf.Clamp01(force / maxForceValue));
            emitterParamsDefault.startSize = size;
            emitterParamsDefault.position = collision.contacts[0].point;
            part.Emit(emitterParamsDefault, 1);
        }
        else {

        }
    }

    private void OnCollisionStay(Collision collision) {
        if (active) {
            float force = collision.impulse.magnitude;
            float val = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(force / maxForceValue));
            emitterParamsDefault.startColor = new Color(val, val, val);
            float size = Mathf.Lerp(maxSize * 0.7f, maxSize, Mathf.Clamp01(force / maxForceValue));
            emitterParamsDefault.startSize = size;
            emitterParamsDefault.position = collision.contacts[0].point;
            part.Emit(emitterParamsDefault, 1);
        }
        else {

        }
    }

    private void OnCollisionExit(Collision collision) {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetColumn : MonoBehaviour {

    public TargetColumnGenome genome;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(TargetColumnGenome genome, EnvironmentGenome envGenomeRef) {
        this.genome = genome;

        gameObject.GetComponent<Collider>().enabled = false;

        //Vector2 randDir = UnityEngine.Random.insideUnitCircle;
        //float radius = 20f;
        //float x = UnityEngine.Random.Range(genome.minX, genome.maxX) * Challenge.GetChallengeArenaBounds(Challenge.Type.Test).x - (Challenge.GetChallengeArenaBounds(Challenge.Type.Test).x * 0.5f);
        //float z = UnityEngine.Random.Range(genome.minZ, genome.maxZ) * Challenge.GetChallengeArenaBounds(Challenge.Type.Test).z - (Challenge.GetChallengeArenaBounds(Challenge.Type.Test).z * 0.5f);
        float x = genome.minX * Challenge.GetChallengeArenaBounds(Challenge.Type.Test).x - (Challenge.GetChallengeArenaBounds(Challenge.Type.Test).x * 0.5f);
        float z = genome.minZ * Challenge.GetChallengeArenaBounds(Challenge.Type.Test).z - (Challenge.GetChallengeArenaBounds(Challenge.Type.Test).z * 0.5f);

        Vector3 targetPos = new Vector3(x, 0.5f, z);
        Vector3 startToTarget = targetPos - envGenomeRef.agentStartPositionsList[0].agentStartPosition;
        if(startToTarget.magnitude <= 3f) {
            targetPos = new Vector3(envGenomeRef.agentStartPositionsList[0].agentStartPosition.x, 0.5f, envGenomeRef.agentStartPositionsList[0].agentStartPosition.z + 6f);
            //Debug.Log("Moved TargetPos to avoid collision!!!");
        }

        gameObject.transform.localPosition = targetPos;
        gameObject.transform.localScale = new Vector3(genome.targetRadius, 10f, genome.targetRadius);        
    }
}

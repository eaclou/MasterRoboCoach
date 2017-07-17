﻿using System.Collections;
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

    public void Initialize(TargetColumnGenome genome) {
        this.genome = genome;

        gameObject.GetComponent<Collider>().enabled = false;

        //Vector2 randDir = UnityEngine.Random.insideUnitCircle;
        //float radius = 20f;
        float x = UnityEngine.Random.Range(genome.minX, genome.maxX) * Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).x - (Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).x * 0.5f);
        float z = UnityEngine.Random.Range(genome.minZ, genome.maxZ) * Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).z - (Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).z * 0.5f);
        //float x = genome.minX * Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).x - (Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).x * 0.5f);
        //float z = genome.minZ * Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).z - (Challenge.GetChallengeArenaBounds(genome.parentGenome.challengeType).z * 0.5f);

        gameObject.transform.localPosition = new Vector3(x, 0.5f, z);
        gameObject.transform.localScale = new Vector3(genome.targetRadius, 10f, genome.targetRadius);        
    }
}

using UnityEngine;
using System.Collections;

public class ChallengeTest : ChallengeBase {

    //public Vector3 agentPosition;
    //public Vector3 targetPosition;
    //public float targetRadius;

    public Agent agent;
    public Environment environment;

    // Constructor
    public ChallengeTest() {
        this.challengeType = Challenge.Type.Test;
    }

    public void HookUpModules() {
        for (int i = 0; i < agent.targetSensorList.Count; i++) {
            agent.targetSensorList[i].targetPosition = environment.targetColumn.gameObject.transform;
        }
    }
}

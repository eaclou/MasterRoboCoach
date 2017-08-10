using UnityEngine;
using System.Collections;

public class ChallengeTest : ChallengeBase {

    // Constructor
    public ChallengeTest() {
        this.challengeType = Challenge.Type.Test;
    }

    public override void HookUpModules() {
        for (int i = 0; i < agent.targetSensorList.Count; i++) {
            agent.targetSensorList[i].targetPosition = environment.environmentGameplay.targetColumn.gameObject.transform;
        }
    }
}

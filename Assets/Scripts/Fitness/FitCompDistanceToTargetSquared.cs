using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompDistanceToTargetSquared : FitCompBase {

    public Vector3[] pointA;
    public Vector3[] pointB;
	
    public FitCompDistanceToTargetSquared(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;

        pointA = new Vector3[1];
        pointB = new Vector3[1];
    }

    public override void TickScore() {
        float distSquared = (pointB[0] - pointA[0]).sqrMagnitude;
        switch(sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += distSquared;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, distSquared);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, distSquared);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = distSquared;
                break;
            default:
                break;
        }
        //Debug.Log("distSquared: " + distSquared.ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompCustom : FitCompBase {

    public float custom;
    

    public FitCompCustom(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;

        //pointA = new Vector3[1];
        //pointB = new Vector3[1];
    }

    public override void TickScore() {
        //float distSquared = (pointB - pointA).sqrMagnitude;
        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += custom;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, custom);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, custom);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = custom;
                break;
            default:
                break;
        }
        //Debug.Log("distSquared: " + distSquared.ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompAltitude : FitCompBase {

    public float altitude;
    

    public FitCompAltitude(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;

        //pointA = new Vector3[1];
        //pointB = new Vector3[1];
    }

    public override void TickScore() {
        //float distSquared = (pointB - pointA).sqrMagnitude;
        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += altitude;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, altitude);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, altitude);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = altitude;
                break;
            default:
                break;
        }
        //Debug.Log("distSquared: " + distSquared.ToString());
    }
}

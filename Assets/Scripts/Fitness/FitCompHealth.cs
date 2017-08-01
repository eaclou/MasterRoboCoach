using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompHealth : FitCompBase {

    public float health;

    public FitCompHealth(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;
    }

    public override void TickScore() {
        //float velocity = vel.magnitude;
        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += health;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, health);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, health);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = health;
                break;
            default:
                break;
        }
    }
}

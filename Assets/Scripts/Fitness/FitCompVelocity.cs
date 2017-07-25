using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompVelocity : FitCompBase {

    public Vector3 vel;

    public FitCompVelocity(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;                
    }

    public override void TickScore() {
        float velocity = vel.magnitude;
        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Average:
                rawScore += velocity;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, velocity);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, velocity);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = velocity;
                break;
            default:
                break;
        }
    }
}

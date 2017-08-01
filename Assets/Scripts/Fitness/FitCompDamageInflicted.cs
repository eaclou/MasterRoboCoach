using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompDamageInflicted : FitCompBase {

    public float damage;

    public FitCompDamageInflicted(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;
    }

    public override void TickScore() {
        //float velocity = vel.magnitude;
        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += damage;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, damage);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, damage);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = damage;
                break;
            default:
                break;
        }
    }
}

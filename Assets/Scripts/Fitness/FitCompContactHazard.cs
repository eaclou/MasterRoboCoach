using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompContactHazard : FitCompBase {

    public bool contactingHazard;

    public FitCompContactHazard(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;
    }

    public override void TickScore() {
        float contact;
        if(contactingHazard) {
            contact = 1f;
        }
        else {
            contact = 0f;
        }
        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += contact;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, contact);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, contact);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = contact;
                break;
            default:
                break;
        }
    }
}

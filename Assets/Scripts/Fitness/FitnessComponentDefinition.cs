using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum FitnessComponentType {
    Distance,
    DistanceSquared,
    Velocity
};
[System.Serializable]
public enum FitnessComponentMeasure {
    Average,
    Min,
    Max,
    Last
};

[System.Serializable]
public class FitnessComponentDefinition {
    // This Class serves as a serializable list of attributes and types for each fitness component
    // within a population's FitnessManager, to let it know which FitnessComponent Types to instantiate
    // during an Evaluation.
    
    public FitnessComponentType type;
    public FitnessComponentMeasure measure;
    public float weight;


    public FitnessComponentDefinition(FitnessComponentType type, FitnessComponentMeasure measure, float weight) {
        this.type = type;
        this.measure = measure;
        this.weight = weight;
    }
}

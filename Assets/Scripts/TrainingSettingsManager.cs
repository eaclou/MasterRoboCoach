﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainingSettingsManager {

    public float mutationChance;
    public float mutationStepSize;

	public TrainingSettingsManager(float mutationChance, float mutationStepSize) {
        this.mutationChance = mutationChance;
        this.mutationStepSize = mutationStepSize;
    }
}

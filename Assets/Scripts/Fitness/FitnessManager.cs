using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessManager {

    public float[] rankedFitnessList;
    public int[] rankedIndicesList;

    // TEMP:
    public List<float> rawFitnessScores;

	public FitnessManager() {

    }

    public void ResetFitnessScores(int populationSize) {
        if(rawFitnessScores == null) {
            rawFitnessScores = new List<float>();
            for(int i = 0; i < populationSize; i++) {
                rawFitnessScores.Add(0f);
            }
        }
        else {
            if (populationSize != rawFitnessScores.Count) {
                rawFitnessScores = new List<float>();
                for (int i = 0; i < populationSize; i++) {
                    rawFitnessScores.Add(0f);
                }
            }
            else {
                for (int i = 0; i < populationSize; i++) {
                    rawFitnessScores[i] = 0f;
                }
            }
        }        
    }

    public void ProcessAndRankRawFitness() {
        rankedIndicesList = new int[rawFitnessScores.Count];
        rankedFitnessList = new float[rawFitnessScores.Count];

        // populate arrays:
        for (int i = 0; i < rawFitnessScores.Count; i++) {
            rankedIndicesList[i] = i;
            rankedFitnessList[i] = rawFitnessScores[i];
        }
        for (int i = 0; i < rawFitnessScores.Count - 1; i++) {
            for (int j = 0; j < rawFitnessScores.Count - 1; j++) {
                float swapFitA = rankedFitnessList[j];
                float swapFitB = rankedFitnessList[j + 1];
                int swapIdA = rankedIndicesList[j];
                int swapIdB = rankedIndicesList[j + 1];

                if (swapFitA > swapFitB) {
                    rankedFitnessList[j] = swapFitB;
                    rankedFitnessList[j + 1] = swapFitA;
                    rankedIndicesList[j] = swapIdB;
                    rankedIndicesList[j + 1] = swapIdA;
                }
            }
        }
        string fitnessRankText = "";
        for (int i = 0; i < rawFitnessScores.Count; i++) {
            fitnessRankText += "[" + rankedIndicesList[i].ToString() + "]: " + rankedFitnessList[i].ToString() + "\n";
        }
        Debug.Log(fitnessRankText);
    }
}

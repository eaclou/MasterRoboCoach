using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FitnessManager {

    // The list of types of FitnessComponents that are currently active and their weights:
    public List<FitnessComponentDefinition> fitnessComponentDefinitions;
    public List<FitnessComponentDefinition> pendingFitnessComponentDefinitions; // edited by UI, applied each new generation

    public float[] rankedFitnessList;
    public int[] rankedIndicesList;
    public float[] processedFitnessScores;
    // TEMP OLD:
    //public List<float> rawFitnessScores;
    // New:
    // Outer list is all the population's genomes
    // Inner list contains group of fitComp's per evaluation instance for that genome
    public List<FitnessComponentEvaluationGroup>[] FitnessEvalGroupArray; // each index is one genome

	public FitnessManager() {
        fitnessComponentDefinitions = new List<FitnessComponentDefinition>();
        pendingFitnessComponentDefinitions = new List<FitnessComponentDefinition>();
    }
   
    public void InitializeForNewGeneration(int populationSize) {
        //Debug.Log("InitializeForNewGeneration: " + populationSize.ToString());
        if (FitnessEvalGroupArray == null) {
            FitnessEvalGroupArray = new List<FitnessComponentEvaluationGroup>[populationSize];
            for (int i = 0; i < populationSize; i++) { // for each genome:                
                FitnessEvalGroupArray[i] = new List<FitnessComponentEvaluationGroup>(); // The list of fitness Component Groups
            }
        }
        else {  // Not null
            if(FitnessEvalGroupArray.Length != populationSize) {  // but incorrectly sized - population size has changed
                FitnessEvalGroupArray = new List<FitnessComponentEvaluationGroup>[populationSize];
            }            
            for(int i = 0; i < populationSize; i++) { // for each genome:
                if (FitnessEvalGroupArray[i] != null) {
                    FitnessEvalGroupArray[i].Clear();
                }
                else {
                    FitnessEvalGroupArray[i] = new List<FitnessComponentEvaluationGroup>(); // The list of fitness Component Groups                    
                }                
            }
        }

        // copy pending changes into main List.
        SetFitnessFunctionFromPending();
        // then update pending list
    }

    public void SetFitnessFunctionFromPending() {
        if (fitnessComponentDefinitions == null) {
            fitnessComponentDefinitions = new List<FitnessComponentDefinition>();
        }
        else {
            fitnessComponentDefinitions.Clear();
        }
        for (int i = 0; i < pendingFitnessComponentDefinitions.Count; i++) {
            FitnessComponentDefinition definitionCopy = new FitnessComponentDefinition(pendingFitnessComponentDefinitions[i].type, pendingFitnessComponentDefinitions[i].measure, pendingFitnessComponentDefinitions[i].weight, pendingFitnessComponentDefinitions[i].biggerIsBetter);
            fitnessComponentDefinitions.Add(definitionCopy);
        }
    }
    public void SetPendingFitnessListFromMaster() {
        if(pendingFitnessComponentDefinitions == null) {
            pendingFitnessComponentDefinitions = new List<FitnessComponentDefinition>();
        }
        else {
            pendingFitnessComponentDefinitions.Clear();
        }
        for(int i = 0; i < fitnessComponentDefinitions.Count; i++) {
            FitnessComponentDefinition definitionCopy = new FitnessComponentDefinition(fitnessComponentDefinitions[i].type, fitnessComponentDefinitions[i].measure, fitnessComponentDefinitions[i].weight, fitnessComponentDefinitions[i].biggerIsBetter);
            pendingFitnessComponentDefinitions.Add(definitionCopy);
        }
    }

    public void AddNewFitCompEvalGroup(FitnessComponentEvaluationGroup evalGroup, int genomeIndex) {
        FitnessComponentEvaluationGroup groupCopy = new FitnessComponentEvaluationGroup();  // Wrapper class is new memory address...
        for(int i = 0; i < evalGroup.fitCompList.Count; i++) {
            groupCopy.fitCompList.Add(evalGroup.fitCompList[i]);  // ... but they share the actual FitComp objects as ref
        }
        //Debug.Log("FitnessEvalGroupArray? " + FitnessEvalGroupArray[0].ToString());
        FitnessEvalGroupArray[genomeIndex].Add(groupCopy);
    }

    public void ProcessAndRankRawFitness() {
        ProcessRawFitness();
        RankProcessedFitness();
    }

    public void ProcessRawFitness() {
        int popSize = FitnessEvalGroupArray.Length;

        if(processedFitnessScores == null) {
            processedFitnessScores = new float[popSize];
        }
        else {
            if(processedFitnessScores.Length != popSize) {
                processedFitnessScores = new float[popSize];
            }
        }

        // Process!  -- Makes a lot of assumptions!!!
        int numComponents = fitnessComponentDefinitions.Count;
        float[] componentRecordMinimums = new float[numComponents];
        float[] componentRecordMaximums = new float[numComponents];
        float[] componentWeightsNormalized = new float[numComponents];
        
        // Loop through all scores and find the total range of values for this generation in order to normalize them
        for(int i = 0; i < numComponents; i++) {  // for each fitness component
            componentRecordMinimums[i] = float.PositiveInfinity;
            componentRecordMaximums[i] = float.NegativeInfinity;
            for (int j = 0; j < FitnessEvalGroupArray.Length; j++) { // loop through each genome
                for(int k = 0; k < FitnessEvalGroupArray[j].Count; k++) { // for each EvalGroup of this genome
                    float score = FitnessEvalGroupArray[j][k].fitCompList[i].rawScore;
                    // Won't work if evalTimes are different? -- among other things...
                    componentRecordMinimums[i] = Mathf.Min(score, componentRecordMinimums[i]);
                    componentRecordMaximums[i] = Mathf.Max(score, componentRecordMaximums[i]);
                }
            }
        }
        // Loop through components and get normalized weights:
        float totalFitCompWeight = 0f; 
        for(int f = 0; f < numComponents; f++) {
            totalFitCompWeight += fitnessComponentDefinitions[f].weight;
        }
        // store normalized component weights:
        for (int f = 0; f < numComponents; f++) {
            componentWeightsNormalized[f] = Mathf.Clamp01(fitnessComponentDefinitions[f].weight / totalFitCompWeight);
            //temp
            //Debug.Log("component[" + f.ToString() + "] min: " + componentRecordMinimums[f].ToString() + ", max: " + componentRecordMaximums[f].ToString());
        }
        // Loop through all genomes and tally up their scores from each of their evaluations
        for (int a = 0; a < FitnessEvalGroupArray.Length; a++) {  // a = genomes
            float genomeScore = 0f;
            for(int b = 0; b < FitnessEvalGroupArray[a].Count; b++) {  // b = evalGroups
                float evalScore = 0f;
                for(int c = 0; c < FitnessEvalGroupArray[a][b].fitCompList.Count; c++) {  // c = fitnessComponents
                    if((componentRecordMaximums[c] - componentRecordMinimums[c]) > 0f) {
                        float normalizedScore = (FitnessEvalGroupArray[a][b].fitCompList[c].rawScore - componentRecordMinimums[c]) / (componentRecordMaximums[c] - componentRecordMinimums[c]);
                        if(FitnessEvalGroupArray[a][b].fitCompList[c].sourceDefinition.biggerIsBetter) {
                            normalizedScore = 1f - normalizedScore;
                        }
                        evalScore += normalizedScore * componentWeightsNormalized[c];
                    }
                    else {
                        evalScore += 0f;
                    }                    
                }
                genomeScore += evalScore;
            }
            genomeScore /= FitnessEvalGroupArray[a].Count;
            
            // Store final processed score in global array
            processedFitnessScores[a] = genomeScore;
        }
    }

    public void RankProcessedFitness() {
        rankedIndicesList = new int[processedFitnessScores.Length];
        rankedFitnessList = new float[processedFitnessScores.Length];
        
        // populate arrays:
        for (int i = 0; i < processedFitnessScores.Length; i++) {
            rankedIndicesList[i] = i;
            rankedFitnessList[i] = processedFitnessScores[i];
        }
        for (int i = 0; i < processedFitnessScores.Length - 1; i++) {
            for (int j = 0; j < processedFitnessScores.Length - 1; j++) {
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
        /*string fitnessRankText = "";
        for (int i = 0; i < processedFitnessScores.Length; i++) {
            fitnessRankText += "[" + rankedIndicesList[i].ToString() + "]: " + rankedFitnessList[i].ToString() + "\n";
        }
        Debug.Log(fitnessRankText);
        */
        
        // TEMP!
        AddTopHalfBonus();
    }

    public void AddTopHalfBonus() {
        for(int i = 0; i < rankedFitnessList.Length; i++) {
            if(i*2 < rankedFitnessList.Length) {
                rankedFitnessList[rankedIndicesList[i]] = 0f;
            }
        }
    }

    public int GetAgentIndexByLottery() {
        int selectedIndex = 0;
        // calculate total fitness of all agents
        float totalFitness = 0f;
        for(int i = 0; i < rankedFitnessList.Length; i++) {
            totalFitness += (1f - rankedFitnessList[i]);  // Fitness right now is Lower = Better, so take inverse! Might want to change this...
        }
        // generate random lottery value between 0f and totalFitness:
        float lotteryValue = UnityEngine.Random.Range(0f, totalFitness);
        float currentValue = 0f;
        for (int i = 0; i < rankedFitnessList.Length; i++) {
            if(lotteryValue >= currentValue && lotteryValue < (currentValue + (1f - rankedFitnessList[i]))) {
                // Jackpot!
                selectedIndex = rankedIndicesList[i];
                //Debug.Log("Selected: " + selectedIndex.ToString() + "! (" + i.ToString() + ") fit= " + currentValue.ToString() + "--" + (currentValue + (1f - rankedFitnessList[i])).ToString() + " / " + totalFitness.ToString() + ", lotto# " + lotteryValue.ToString() + ", fit= " + (1f - rankedFitnessList[i]).ToString());
            }
            currentValue += (1f - rankedFitnessList[i]); // add this agent's fitness to current value for next check            
        }

        return selectedIndex;
    }
}

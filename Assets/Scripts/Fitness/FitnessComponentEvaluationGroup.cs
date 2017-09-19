using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessComponentEvaluationGroup {
    
    public List<FitCompBase> fitCompList;
	
    public FitnessComponentEvaluationGroup() {
        fitCompList = new List<FitCompBase>();
    }

    public void CreateFitnessComponentEvaluationGroup(FitnessManager fitnessManager, int genomeIndex) {

        for (int i = 0; i < fitnessManager.fitnessComponentDefinitions.Count; i++) {
            switch (fitnessManager.fitnessComponentDefinitions[i].type) {
                case FitnessComponentType.DistanceToTargetSquared:
                    FitCompDistanceToTargetSquared fitCompDistanceToTargetSquared = new FitCompDistanceToTargetSquared(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompDistanceToTargetSquared);
                    break;
                case FitnessComponentType.Velocity:
                    FitCompVelocity fitCompVelocity = new FitCompVelocity(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompVelocity);
                    break;
                case FitnessComponentType.ContactHazard:
                    FitCompContactHazard fitCompContactHazard = new FitCompContactHazard(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompContactHazard);
                    break;
                case FitnessComponentType.DamageInflicted:
                    FitCompDamageInflicted fitCompDamageInflicted = new FitCompDamageInflicted(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompDamageInflicted);
                    break;
                case FitnessComponentType.Health:
                    FitCompHealth fitCompHealth = new FitCompHealth(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompHealth);
                    break;
                case FitnessComponentType.Random:
                    FitCompRandom fitCompRandom = new FitCompRandom(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompRandom);
                    break;
                case FitnessComponentType.WinLoss:
                    FitCompWinLoss fitCompWinLoss = new FitCompWinLoss(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompWinLoss);
                    break;
                case FitnessComponentType.DistToOrigin:
                    FitCompDistFromOrigin fitCompDistFromOrigin = new FitCompDistFromOrigin(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompDistFromOrigin);
                    break;
                case FitnessComponentType.Altitude:
                    FitCompAltitude fitCompAltitude = new FitCompAltitude(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompAltitude);
                    break;
                case FitnessComponentType.Custom:
                    FitCompCustom fitCompCustom = new FitCompCustom(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompCustom);
                    break;
                default:
                    Debug.LogError("No such component type! (" + fitnessManager.fitnessComponentDefinitions[i].type.ToString() + ")");
                    break;
            }
        }

        fitnessManager.AddNewFitCompEvalGroup(this, genomeIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper class to allow edit-time setup of AgentGenomes

[CreateAssetMenuAttribute(fileName = "AgentGenomeTemplate", menuName = "AgentGenomeTemplates/New", order = 0)]
public class AgentBodyGenomeTemplate : ScriptableObject {

    
    public BodyGenome bodyGenome;

    void Awake() {
        Debug.Log("AgentGenomeTemplate Awake!");
    }

    void OnDestroy() {
        Debug.Log("AgentGenomeTemplate OnDestroy!");
    }

    void OnDisable() {
        Debug.Log("AgentGenomeTemplate OnDisable!");
    }

    void OnEnable() {
        Debug.Log("AgentGenomeTemplate OnEnable!");
    }

    public static string GetAgentBodyTypeURL(AgentBodyType type) {
        string bodyURL;
        switch(type) {
            case AgentBodyType.HoverBot:
                bodyURL = "Prefabs/AgentPrefabs/AgentRoombot";
                break;
            case AgentBodyType.TelevisionWalker:
                bodyURL = "Prefabs/AgentPrefabs/AgentTelevisionWalker";
                break;
            case AgentBodyType.DogCar:
                bodyURL = "Prefabs/AgentPrefabs/AgentDogCar";
                break;
            case AgentBodyType.CombatBot:
                bodyURL = "Prefabs/AgentPrefabs/AgentCombatBot";
                break;
            case AgentBodyType.Unicycle:
                bodyURL = "Prefabs/AgentPrefabs/AgentUnicycle";
                break;
            case AgentBodyType.BipedA:
                bodyURL = "Prefabs/AgentPrefabs/AgentBipedA";
                break;
            case AgentBodyType.TripodA:
                bodyURL = "Prefabs/AgentPrefabs/AgentTripodA";
                break;
            case AgentBodyType.HexapodA:
                bodyURL = "Prefabs/AgentPrefabs/AgentHexapodA";
                break;
            case AgentBodyType.AbsWheelA:
                bodyURL = "Prefabs/AgentPrefabs/AgentAbsWheelA";
                break;
            case AgentBodyType.SphereShipA:
                bodyURL = "Prefabs/AgentPrefabs/AgentSphereShipA";
                break;
            default:
                bodyURL = "";
                Debug.LogError("NO BODYURL FOUND");
                break;
        }
        return bodyURL;
    }
}

public enum AgentBodyType {
    HoverBot,
    TelevisionWalker,
    DogCar,
    CombatBot,
    Unicycle,
    BipedA,
    TripodA,
    HexapodA,
    AbsWheelA,
    SphereShipA
}

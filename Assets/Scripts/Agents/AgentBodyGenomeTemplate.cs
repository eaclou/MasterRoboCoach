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
    CombatBot
}

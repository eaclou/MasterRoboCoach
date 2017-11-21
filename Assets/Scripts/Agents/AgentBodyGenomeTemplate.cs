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
            case AgentBodyType.AbsWheelA:
                bodyURL = "Prefabs/AgentPrefabs/AgentAbsWheelA";
                break;
            case AgentBodyType.BipedA:
                bodyURL = "Prefabs/AgentPrefabs/AgentBipedA";
                break;
            case AgentBodyType.CombatBotA:
                bodyURL = "Prefabs/AgentPrefabs/AgentCombatBotA";
                break;
            case AgentBodyType.DogCarA:
                bodyURL = "Prefabs/AgentPrefabs/AgentDogCarA";
                break;
            case AgentBodyType.GroundRollerA:
                bodyURL = "Prefabs/AgentPrefabs/AgentGroundRollerA";
                break;
            case AgentBodyType.GroundRollerB:
                bodyURL = "Prefabs/AgentPrefabs/AgentGroundRollerB";
                break;
            case AgentBodyType.HexapodA:
                bodyURL = "Prefabs/AgentPrefabs/AgentHexapodA";
                break;
            case AgentBodyType.HoverBotA:
                bodyURL = "Prefabs/AgentPrefabs/AgentHoverBotA";
                break;
            case AgentBodyType.HoverBotB:
                bodyURL = "Prefabs/AgentPrefabs/AgentHoverBotB";
                break;
            case AgentBodyType.PistonWalkerA:
                bodyURL = "Prefabs/AgentPrefabs/AgentPistonWalkerA";
                break;
            case AgentBodyType.QuadrapedA:
                bodyURL = "Prefabs/AgentPrefabs/AgentQuadrapedA";
                break;
            case AgentBodyType.QuadrapedB:
                bodyURL = "Prefabs/AgentPrefabs/AgentQuadrapedB";
                break;
            case AgentBodyType.QuadrapedC:
                bodyURL = "Prefabs/AgentPrefabs/AgentQuadrapedC";
                break;
            case AgentBodyType.SlapperTurtleA:
                bodyURL = "Prefabs/AgentPrefabs/AgentSlapperTurtleA";
                break;
            case AgentBodyType.SnakeA:
                bodyURL = "Prefabs/AgentPrefabs/AgentSnakeA";
                break;
            case AgentBodyType.SphereA:
                bodyURL = "Prefabs/AgentPrefabs/AgentSphereA";
                break;
            case AgentBodyType.SphereShipA:
                bodyURL = "Prefabs/AgentPrefabs/AgentSphereShipA";
                break;
            case AgentBodyType.TelevisionWalkerA:
                bodyURL = "Prefabs/AgentPrefabs/AgentTelevisionWalkerA";
                break;
            case AgentBodyType.TelevisionWalkerB:
                bodyURL = "Prefabs/AgentPrefabs/AgentTelevisionWalkerB";
                break;
            case AgentBodyType.TripodA:
                bodyURL = "Prefabs/AgentPrefabs/AgentTripodA";
                break;
            case AgentBodyType.UnicycleA:
                bodyURL = "Prefabs/AgentPrefabs/AgentUnicycleA";
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
    AbsWheelA,
    BipedA,
    CombatBotA,
    DogCarA,
    GroundRollerA,
    GroundRollerB,
    HexapodA,
    HoverBotA,
    HoverBotB,
    PistonWalkerA,
    QuadrapedA,
    QuadrapedB,
    QuadrapedC,
    SlapperTurtleA,
    SnakeA,
    SphereA,
    SphereShipA,
    TelevisionWalkerA,
    TelevisionWalkerB,    
    TripodA,
    UnicycleA
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper class to allow edit-time setup of AgentGenomes

[CreateAssetMenuAttribute(fileName = "AgentGenomeTemplate", menuName = "AgentGenomeTemplates/New", order = 0)]
public class AgentGenomeTemplate : ScriptableObject {

    public GameObject templateBody;
    public AgentGenome templateGenome;

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

}

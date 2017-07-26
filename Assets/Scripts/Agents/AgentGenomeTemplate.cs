using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper class to allow edit-time setup of AgentGenomes

[CreateAssetMenuAttribute(fileName = "AgentGenomeTemplate", menuName = "AgentGenomeTemplates/New", order = 0)]
public class AgentGenomeTemplate : ScriptableObject {

    public AgentGenome templateGenome;

}

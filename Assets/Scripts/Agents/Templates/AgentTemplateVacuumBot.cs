using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper class to allow edit-time setup of AgentGenomes

[CreateAssetMenuAttribute(fileName = "TemplateVacuumBot", menuName = "AgentGenomeTemplates/VacuumBot", order = 0)]
public class AgentTemplateVacuumBot : ScriptableObject {

    public AgentGenome templateGenome;

}

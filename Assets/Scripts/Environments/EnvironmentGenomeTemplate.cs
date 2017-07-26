using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper class to allow edit-time setup of EnvironmentGenomes

[CreateAssetMenuAttribute(fileName = "EnvironmentGenomeTemplate", menuName = "EnvironmentGenomeTemplates/New", order = 10000)]
public class EnvironmentGenomeTemplate : ScriptableObject {

    public EnvironmentGenome templateGenome;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeBase : MonoBehaviour {

    public Agent agent;
    public Environment environment;
    public Challenge.Type challengeType;

    public virtual void HookUpModules() {
        
    }
}

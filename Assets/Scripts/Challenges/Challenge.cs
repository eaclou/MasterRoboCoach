using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge {



	public enum Type {
        Test,
        Racing,
        Combat
    };

    public static Vector3 GetChallengeArenaBounds(Challenge.Type challengeType) {
        Vector3 arenaBounds = new Vector3(10f, 10f, 10f);

        switch (challengeType) {
            case Challenge.Type.Test:
                arenaBounds.x = 40f;
                arenaBounds.y = 40f;
                arenaBounds.z = 40f;

                break;
            case Challenge.Type.Racing:
                arenaBounds.x = 40f;
                arenaBounds.y = 40f;
                arenaBounds.z = 40f;

                break;
            case Challenge.Type.Combat:
                arenaBounds.x = 40f;
                arenaBounds.y = 40f;
                arenaBounds.z = 40f;

                break;
            default:
                //print("default");
                break;
        }

        return arenaBounds;
    }
}

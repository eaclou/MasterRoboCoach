using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationPair {

    public EvaluationStatus status;
	public enum EvaluationStatus {
        Pending,
        InProgress,
        PendingComplete,
        Complete
    };

    public ManualSelectStatus manualSelectStatus;
    public enum ManualSelectStatus {
        Pending,
        Keep,
        Auto,
        Kill,
        Replay
    };

    public int[] evalPairIndices;
    public int focusPopIndex = -1;  // Which Population in this pair is being evaluated?

    public EvaluationPair(int[] pairIndices, int focus) {
        status = EvaluationStatus.Pending;
        evalPairIndices = pairIndices;
        focusPopIndex = focus;
    }
}

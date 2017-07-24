﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationTicket {
    // This is a class that holds the information for an Evaluation Request
    // it needs information about what/which environment & agents are involved
    // Also needs to pass some information to the training instance

    // What data does a ticket need?
    // ID of each Player's AgentGenome
    // ID of EnvironmentGenome
    //     This might also hold data such as: which terrain, Arena bounds, etc.?
    // Game Settings -- maximum evaluation time, start position, goal conditions, abort conditions?

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

    //public int environmentRepIndex = -1;
    public int[] genomeIndices;
    public int focusPopIndex = -1;

    public int maxTimeSteps;
    // public Vector3 ArenaBounds?
    //public ChallengeSettingsGenome challengeSettings;

    public EvaluationTicket(int[] genomeIndices, int focusIndex, int maxTimeSteps) {
        status = EvaluationStatus.Pending;
        //environmentRepIndex = environmentIndex;
        this.genomeIndices = genomeIndices;
        this.focusPopIndex = focusIndex;
        this.maxTimeSteps = maxTimeSteps;
        //challengeSettings = settings;
    }
}

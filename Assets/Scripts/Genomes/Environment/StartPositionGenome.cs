using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StartPositionGenome {
    public Vector3 agentStartPosition;
    public Quaternion agentStartRotation;

    public StartPositionGenome(StartPositionGenome templateGenome) {
        agentStartPosition = new Vector3(templateGenome.agentStartPosition.x, templateGenome.agentStartPosition.y, templateGenome.agentStartPosition.z);
        agentStartRotation = new Quaternion(templateGenome.agentStartRotation.x, templateGenome.agentStartRotation.y, templateGenome.agentStartRotation.z, templateGenome.agentStartRotation.w);
    }

    public void InitializeRandomGenome() {
        // Start positions:
        /*agentStartPositionsList = new List<Vector3>();
        agentStartRotationsList = new List<Quaternion>();
        Vector3 startPosPlayer1 = new Vector3(0f, 0f, 0f);
        Quaternion startRotPlayer1 = Quaternion.identity;
        agentStartPositionsList.Add(startPosPlayer1);
        agentStartRotationsList.Add(startRotPlayer1);
        // TEMP HACKY!!!
        if (challengeType == Challenge.Type.Racing) {
            agentStartPositionsList[0] = new Vector3(0f, 0f, -18f);
        }
        if (challengeType == Challenge.Type.Combat) {
            agentStartPositionsList[0] = new Vector3(0f, 0f, -18f);

            Vector3 startPosPlayer2 = new Vector3(0f, 0f, 18f);
            Quaternion startRotPlayer2 = Quaternion.Euler(0f, 180f, 0f);

            agentStartPositionsList.Add(startPosPlayer2);
            agentStartRotationsList.Add(startRotPlayer2);
        }
        */
    }
}

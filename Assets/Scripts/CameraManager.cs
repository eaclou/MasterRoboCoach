﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    // Camera: -- need to break this out away from TrainingManager
    public GameObject mainCamGroup;
    public GameObject mainCam;
    public int currentCameraMode = 0;  // 0 = wide, 1 = top-down, 2 = shoulder-cam
    public Vector3 cameraPosWide;
    public Quaternion cameraRotWide;
    public Vector3 cameraPosTop;
    public Quaternion cameraRotTop;
    public Vector3 cameraPosShoulder;
    public Quaternion cameraRotShoulder;

    public Vector3 targetCamGroupPosition;
    public Quaternion targetCamGroupRotation;
    public Vector3 targetMainCamPosition;
    public Quaternion targetMainCamRotation;

    private Vector3 focusPoint = Vector3.zero;
    private Vector3 focusPointAvgVel = Vector3.zero;

    public float cameraSpeed = 0.025f;
    public float cameraSpeedAngle = 0.990f;
    public float camSpeedOther = 0.05f;

    // Use this for initialization
    void Start () {
        cameraPosWide = new Vector3(0f, 20f, -30f);
        cameraRotWide = Quaternion.Euler(25f, 0f, 0f);
        cameraPosTop = new Vector3(0f, 40f, 0f);
        cameraRotTop = Quaternion.Euler(90f, 0f, 0f);
        cameraPosShoulder = new Vector3(0f, 1.5f, -2.5f);
        cameraRotShoulder = Quaternion.Euler(12.5f, 0f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void CycleCameraMode() {
        currentCameraMode++;

        if (currentCameraMode > 2) {
            currentCameraMode = 0;
        }
    }

    public void UpdateCameraState(Vector3 focusPos) {
        Vector3 prevFocusPoint = focusPoint;
        Vector3 prevTargetMainCamPos = targetMainCamPosition;

        this.focusPoint = focusPos;
        if (currentCameraMode == 0) {  // wide
            targetCamGroupPosition = Vector3.zero;
            //targetCamGroupRotation = Quaternion.identity;
            targetMainCamPosition = cameraPosWide;
            targetMainCamRotation = cameraRotWide;

            //mainCamGroup.transform.position = new Vector3(0f, 0f, 0f);
            //mainCamGroup.transform.rotation = Quaternion.identity;
            //mainCam.transform.localPosition = cameraPosWide;
            //mainCam.transform.rotation = cameraRotWide;
        }
        else if (currentCameraMode == 1) {  // topdown
            targetCamGroupPosition = Vector3.zero;
            //targetCamGroupRotation = Quaternion.identity;
            targetMainCamPosition = cameraPosTop;
            targetMainCamRotation = cameraRotTop;

            //mainCamGroup.transform.position = new Vector3(0f, 0f, 0f);
            //mainCamGroup.transform.rotation = Quaternion.identity;
            //mainCam.transform.localPosition = cameraPosTop;
            //mainCam.transform.rotation = cameraRotTop;
        }
        else {  // shoulder            
            //targetCamGroupPosition = focusPoint;
            
            targetMainCamPosition = cameraPosShoulder;
            targetMainCamRotation = cameraRotShoulder;

            targetCamGroupPosition = Vector3.Lerp(targetCamGroupPosition, focusPoint, cameraSpeed);

            Vector3 focusPointVel = focusPoint - prevFocusPoint;
            if (focusPointVel.sqrMagnitude == 0f) {
                focusPointVel = new Vector3(0f, 0f, 1f);
            }
            focusPointAvgVel = Vector3.Lerp(focusPoint - prevFocusPoint, focusPointAvgVel, cameraSpeedAngle);
            focusPointAvgVel.y = 0f;
            if(focusPointAvgVel.sqrMagnitude < 0.00001f) {
                focusPointAvgVel = new Vector3(0f, 0f, 1f);
                //Debug.Log("zero!");
            }
            
            //targetCamGroupRotation = Quaternion.LookRotation(focusPointAvgVel.normalized, new Vector3(0f, 1f, 0f));


            //mainCamGroup.transform.position = focusPoint;            
            //mainCam.transform.localPosition = cameraPosShoulder;
            //mainCam.transform.localRotation = cameraRotShoulder;
        }

        MoveTowardsRestPosition();
    }
    
    private void MoveTowardsRestPosition() {
        Vector3 camGroupToTarget = targetCamGroupPosition - mainCamGroup.transform.position;
        Vector3 mainCamToTarget = targetMainCamPosition - mainCam.transform.localPosition;

        Vector3 camGroupMove = camGroupToTarget * camSpeedOther;
        Vector3 mainCamMove = mainCamToTarget * camSpeedOther;

        Quaternion newCamGroupRotation = Quaternion.Lerp(mainCamGroup.transform.rotation, targetCamGroupRotation, camSpeedOther);
        Quaternion newMainCamRotation = Quaternion.Lerp(mainCam.transform.localRotation, targetMainCamRotation, camSpeedOther);

        mainCamGroup.transform.position += camGroupMove;
        mainCam.transform.localPosition += mainCamMove;

        mainCamGroup.transform.rotation = newCamGroupRotation;
        mainCam.transform.localRotation = newMainCamRotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RaycastSensorGenome {
    public int parentID;
    public int inno;

    public RaycastSensorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public RaycastSensorGenome(RaycastSensorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }
}

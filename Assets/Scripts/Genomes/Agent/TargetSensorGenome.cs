using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSensorGenome {
    public int parentID;
    public int inno;
	
    public TargetSensorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public TargetSensorGenome(TargetSensorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }
}

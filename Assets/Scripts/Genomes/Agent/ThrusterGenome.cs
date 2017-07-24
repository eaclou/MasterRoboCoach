using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThrusterGenome {
    public int parentID;
    public int inno;

    public ThrusterGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public ThrusterGenome(ThrusterGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }

}

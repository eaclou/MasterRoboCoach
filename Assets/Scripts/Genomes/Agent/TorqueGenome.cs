using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorqueGenome {
    public int parentID;
    public int inno;

    public TorqueGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public TorqueGenome(TorqueGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }

}

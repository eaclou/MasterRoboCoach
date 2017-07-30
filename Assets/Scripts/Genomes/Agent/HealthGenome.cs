using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthGenome {
    public int parentID;
    public int inno;

    public HealthGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public HealthGenome(HealthGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }
}

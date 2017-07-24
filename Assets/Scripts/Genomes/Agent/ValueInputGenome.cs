using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueInputGenome {
    public int parentID;
    public int inno;
    public float val = 1f;

    public ValueInputGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public ValueInputGenome(ValueInputGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.val = template.val;
    }

}

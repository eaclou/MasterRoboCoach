using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueInputGenome {
    public int parentID;
    public int inno;
    public float val = 1f;

    public ValueInputGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

}

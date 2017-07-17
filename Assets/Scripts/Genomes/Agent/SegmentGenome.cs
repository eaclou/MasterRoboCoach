using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentGenome {

    public int parentID = 0;
    public Vector3 scale = new Vector3(1f, 1f, 1f);

    public SegmentGenome(int pid) {
        parentID = pid;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SegmentGenome {

    public CustomMeshID segmentPreset;
    public int parentID = 0;
    public Vector3 scale = new Vector3(1f, 1f, 1f);

    public SegmentGenome(int pid) {
        parentID = pid;
    }

    public SegmentGenome(SegmentGenome template) {
        this.segmentPreset = template.segmentPreset;  // For now this will be shared reference -- Might have to change this later if meshes change!
        this.parentID = template.parentID;
        this.scale = template.scale;
    }
}

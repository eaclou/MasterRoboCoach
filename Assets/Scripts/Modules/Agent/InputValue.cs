using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputValue {
    public int parentID;
    public int inno;
    public float[] value;

	public InputValue(ValueInputGenome genome) {
        Debug.Log("InputValue(ValueInputGenome genome)");
        parentID = genome.parentID; 
        inno = genome.inno;
        value = new float[1];
        value[0] = genome.val;
    }

    public void Initialize(ValueInputGenome genome) {
        //Debug.Log("InputValue(ValueInputGenome genome)");
        //parentID = genome.parentID;
        inno = genome.inno;
        value = new float[1];
        value[0] = genome.val;
    }
}

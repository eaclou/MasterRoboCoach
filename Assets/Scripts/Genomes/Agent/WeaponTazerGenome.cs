using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponTazerGenome {

    public int parentID;
    public int inno;

    public WeaponTazerGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public WeaponTazerGenome(WeaponTazerGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }
}

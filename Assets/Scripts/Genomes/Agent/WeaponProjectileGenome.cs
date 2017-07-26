using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponProjectileGenome {

    public int parentID;
    public int inno;

    public WeaponProjectileGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public WeaponProjectileGenome(WeaponProjectileGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }
}

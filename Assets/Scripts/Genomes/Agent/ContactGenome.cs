﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContactGenome {
    public int parentID;
    public int inno;

    public ContactGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public ContactGenome(ContactGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }
}

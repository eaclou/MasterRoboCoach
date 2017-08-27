﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentRound {

    public int id;
    public List<TournamentMatchup> matchupList;

    public TournamentRound(int id) {
        this.id = id;
        matchupList = new List<TournamentMatchup>();
    }
}

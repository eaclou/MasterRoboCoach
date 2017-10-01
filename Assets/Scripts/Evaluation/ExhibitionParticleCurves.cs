using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitionParticleCurves : MonoBehaviour {

    // Top List: one for each Player
    // Next List: one for each opponent rep combination (env + other players)
    //public Dictionary<string,ParticleSystem> particleDictionary;
    public ParticleSystem singleTrajectoryCurvesPS;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClearAllSystems() {
        /*foreach (KeyValuePair<string, ParticleSystem> particle in particleDictionary) {
            //Now you can access the key and value both separately from this attachStat as:
            //Debug.Log(particle.Key);
            //particle.Value.gameObject.SetActive(false);
            particle.Value.Clear();
        }*/
        singleTrajectoryCurvesPS.Clear();
    }

    public void SetActiveParticle(EvaluationTicket ticket) {
        if (ticket.environmentGenome.index == 0) { // if this instance is testing an agent vs. the Top Environment, record its curves:
            singleTrajectoryCurvesPS.gameObject.layer = LayerMask.NameToLayer("Default");            
        }        

        /*
        foreach (KeyValuePair<string, ParticleSystem> particle in particleDictionary) {
            //Now you can access the key and value both separately from this attachStat as:
            //Debug.Log(particle.Key);
            //particle.Value.gameObject.SetActive(false);
            particle.Value.gameObject.layer = LayerMask.NameToLayer("Hidden");
        }

        int[] indices = new int[ticket.agentGenomesList.Count + 2];
        indices[0] = ticket.focusPopIndex;
        for(int i = 0; i < ticket.agentGenomesList.Count + 1; i++) {
            if(i == 0) {
                indices[i + 1] = ticket.environmentGenome.index;
            }
            else {
                indices[i + 1] = ticket.agentGenomesList[i - 1].index;
            }
            //indices[i + 1] = ticket.genomeIndices[i];
        }
        indices[ticket.focusPopIndex + 1] = 0;
        string txt = "";
        for (int x = 0; x < indices.Length; x++) {
            txt += indices[x].ToString();
        }
        //Debug.Log("particle: " + txt);
        //string test = "";
        //for(int j = 0; j < ticket.genomeIndices.Length; j++) {
        //    test += ticket.genomeIndices[j].ToString();
        //}
        ParticleSystem focusParticle;
        if(particleDictionary.TryGetValue(txt, out focusParticle)) {
            //Debug.Log("Found it! " + txt + ", " + test + ", " + ticket.focusPopIndex.ToString());            
            focusParticle.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else {
            //Debug.Log("FAILED! " + txt);
        }
        */
    }

    public void CreateOneTrueParticleSystem(TeamsConfig teamsConfig) {

        GameObject particleGO = Instantiate(Resources.Load("Prefabs/ParticleSystems/Trajectory")) as GameObject;
        particleGO.name = "theOneTrueCurve";
        particleGO.transform.parent = this.transform;
        singleTrajectoryCurvesPS = particleGO.GetComponent<ParticleSystem>();
        

        /*var children = new List<GameObject>();
        foreach (Transform child in gameObject.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        if (particleDictionary == null) {
            particleDictionary = new Dictionary<string, ParticleSystem>();
        }
        else {
            particleDictionary.Clear();
        }

        int numPlayers = teamsConfig.playersList.Count;

        for(int i = 0; i < numPlayers; i++) {
            // For each Player:

            //for each combination of opponent representatives:
            //List<int[]> indicesList = new List<int[]>();
            // use these as key?

            
            if(numPlayers > 1) {
                // 2 or more players:
                for(int j = 0; j < teamsConfig.environmentPopulation.representativeGenomeList.Count; j++) {
                    // env reps
                    // will only work with 2 players!!!  v v v v v v v  !!!
                    for(int k = 0; k < teamsConfig.playersList[1 - i].representativeGenomeList.Count; k++) {
                        int[] indices = new int[numPlayers + 2]; // focusPop, + env, + players
                        indices[0] = i + 1;  // focusPop index, env = 0, player 1 = 1
                        indices[1] = j;  // environment
                        indices[2] = 0;  // player1, irrelevant
                        indices[3] = 0;  // player2, irrelevant
                        indices[3 - i] = k; // overwrite proper index

                        string txt = "";
                        for (int x = 0; x < indices.Length; x++) {
                            txt += indices[x].ToString();
                        }
                        GameObject particleGO = Instantiate(Resources.Load("Prefabs/ParticleSystems/Trajectory")) as GameObject;
                        particleGO.name = txt;
                        particleGO.transform.parent = this.transform;
                        ParticleSystem particle = particleGO.GetComponent<ParticleSystem>();
                        particleDictionary.Add(txt, particle);

                        //Debug.Log(particleDictionary.TryGetValue(txt, out particle));
                    }
                }
            }
            else {
                // 1 Player!
                for(int j = 0; j < teamsConfig.environmentPopulation.representativeGenomeList.Count; j++) {
                    int[] indices = new int[numPlayers + 2]; // focusPop, + env, + players
                    indices[0] = i+1;  // focusPop index, env = 0, player 1 = 1
                    indices[1] = j;  // environment
                    indices[2] = 0;  // player1, irrelevant

                    string txt = "";
                    for(int x = 0; x < indices.Length; x++) {
                        txt += indices[x].ToString();
                    }
                    GameObject particleGO = Instantiate(Resources.Load("Prefabs/ParticleSystems/Trajectory")) as GameObject;
                    particleGO.name = txt;
                    particleGO.transform.parent = this.transform;
                    ParticleSystem particle = particleGO.GetComponent<ParticleSystem>();
                    particleDictionary.Add(txt, particle);
                }
            }
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGameplay : MonoBehaviour {

    public GameObject groundCollision;    
    public List<GameObject> arenaWalls;
    public List<GameObject> obstacles;
    public TargetColumn targetColumn;
    public Atmosphere atmosphere;
    
    public PhysicMaterial groundPhysicMaterial;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

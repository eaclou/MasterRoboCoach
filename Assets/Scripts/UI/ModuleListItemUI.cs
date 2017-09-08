using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleListItemUI : MonoBehaviour {

    public TrainingManager trainerRef;
    public int moduleIndex;
    public ModuleViewUI moduleViewUI;

    public Button buttonEditModule;
    public Text textModuleName;
    //public Toggle toggleBigIsBetter;
    //public Button buttonMeasure;
    //public Slider sliderWeight;
    //public Text textWeightValue;

    public EnvironmentModuleGenomeType envType;
    public AgentModuleGenomeType agentType;
    public bool isEnv;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData(EnvironmentModuleGenomeType envType, AgentModuleGenomeType agentType, bool isEnv) {
        this.envType = envType;
        this.agentType = agentType;
        this.isEnv = isEnv;

        //textFitnessComponentName.text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].type.ToString();
        //toggleBigIsBetter.isOn = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].biggerIsBetter;
        //buttonMeasure.GetComponentInChildren<Text>().text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure.ToString();
        //sliderWeight.value = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].weight;
        //textWeightValue.text = sliderWeight.value.ToString();
        if(isEnv) {
            switch (envType) {
                case EnvironmentModuleGenomeType.Terrain:
                    // do stuff
                    textModuleName.text = "Terrain " + moduleIndex.ToString();
                    break;
                case EnvironmentModuleGenomeType.Target:
                    // do stuff
                    textModuleName.text = "Target " + moduleIndex.ToString();
                    break;
                case EnvironmentModuleGenomeType.Obstacles:
                    // do stuff
                    textModuleName.text = "Obstacles " + moduleIndex.ToString();
                    break;                
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! EnvironmentModuleGenomeType: " + envType.ToString());
                    break;
            }
        }
        else {
            switch (agentType) {
                case AgentModuleGenomeType.BasicJoint:
                    // do stuff
                    textModuleName.text = "Basic Joint #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.BasicWheel:
                    // do stuff
                    textModuleName.text = "Basic Wheel #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Contact:
                    // do stuff
                    textModuleName.text = "Contact Sensor #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Health:
                    // do stuff
                    textModuleName.text = "Health Sensor #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Oscillator:
                    // do stuff
                    textModuleName.text = "Oscillator Input #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Raycast:
                    // do stuff
                    textModuleName.text = "Raycast Sensor #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Target:
                    // do stuff
                    textModuleName.text = "Target Sensor #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Thruster:
                    // do stuff
                    textModuleName.text = "Thruster #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Torque:
                    // do stuff
                    textModuleName.text = "Torque #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.Value:
                    // do stuff
                    textModuleName.text = "Value Input #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.WeaponProjectile:
                    // do stuff
                    textModuleName.text = "Weapon Projectile #" + moduleIndex.ToString();
                    break;
                case AgentModuleGenomeType.WeaponTazer:
                    // do stuff
                    textModuleName.text = "Weapon Tazer #" + moduleIndex.ToString();
                    break;
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! AgentModuleGenomeType: " + agentType.ToString());
                    break;
            }
        }
        
    }

    public void ClickEdit() {
        string debugText = "";
        if (isEnv) {
            debugText = "ClickEdit(" + envType.ToString() + " " + moduleIndex.ToString() + ")";
        }
        else {
            debugText = "ClickEdit(" + agentType.ToString() + " " + moduleIndex.ToString() + ")";
        }
        Debug.Log(debugText);

        moduleViewUI.ClickEditModule(envType, agentType, isEnv, moduleIndex);
        //fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions.RemoveAt(fitnessIndex);
        //fitnessFunctionUI.SetStatusFromData(trainerRef);
    }
}

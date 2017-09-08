using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyGenome {

    public AgentBodyType bodyType;
    // Modules:
    public List<BasicWheelGenome> basicWheelList;
    public List<BasicJointGenome> basicJointList;
    public List<ContactGenome> contactSensorList;
    public List<HealthGenome> healthModuleList;
    public List<OscillatorGenome> oscillatorInputList;
    public List<RaycastSensorGenome> raycastSensorList;
    public List<TargetSensorGenome> targetSensorList;
    public List<ThrusterGenome> thrusterList;
    public List<TorqueGenome> torqueList;
    public List<ValueInputGenome> valueInputList;
    public List<WeaponProjectileGenome> weaponProjectileList;
    public List<WeaponTazerGenome> weaponTazerList;

    public BodyGenome() {

    }

    public void CopyBodyGenomeFromTemplate(BodyGenome templateGenome) {
        // This method creates a clone of the provided BodyGenome - should have no shared references!!!
        
        bodyType = templateGenome.bodyType;
        // copy module lists:
        basicWheelList = new List<BasicWheelGenome>();
        for (int i = 0; i < templateGenome.basicWheelList.Count; i++) {
            BasicWheelGenome genomeCopy = new BasicWheelGenome(templateGenome.basicWheelList[i]);
            basicWheelList.Add(genomeCopy);
        }
        basicJointList = new List<BasicJointGenome>();
        for (int i = 0; i < templateGenome.basicJointList.Count; i++) {
            BasicJointGenome genomeCopy = new BasicJointGenome(templateGenome.basicJointList[i]);
            basicJointList.Add(genomeCopy);
        }
        contactSensorList = new List<ContactGenome>();
        for (int i = 0; i < templateGenome.contactSensorList.Count; i++) {
            ContactGenome genomeCopy = new ContactGenome(templateGenome.contactSensorList[i]);
            contactSensorList.Add(genomeCopy);
        }
        healthModuleList = new List<HealthGenome>();
        for (int i = 0; i < templateGenome.healthModuleList.Count; i++) {
            HealthGenome genomeCopy = new HealthGenome(templateGenome.healthModuleList[i]);
            healthModuleList.Add(genomeCopy);
        }
        oscillatorInputList = new List<OscillatorGenome>();
        for (int i = 0; i < templateGenome.oscillatorInputList.Count; i++) {
            OscillatorGenome genomeCopy = new OscillatorGenome(templateGenome.oscillatorInputList[i]);
            oscillatorInputList.Add(genomeCopy);
        }
        raycastSensorList = new List<RaycastSensorGenome>();
        for (int i = 0; i < templateGenome.raycastSensorList.Count; i++) {
            RaycastSensorGenome genomeCopy = new RaycastSensorGenome(templateGenome.raycastSensorList[i]);
            raycastSensorList.Add(genomeCopy);
        }
        targetSensorList = new List<TargetSensorGenome>();
        for (int i = 0; i < templateGenome.targetSensorList.Count; i++) {
            TargetSensorGenome genomeCopy = new TargetSensorGenome(templateGenome.targetSensorList[i]);
            targetSensorList.Add(genomeCopy);
        }
        thrusterList = new List<ThrusterGenome>();
        for (int i = 0; i < templateGenome.thrusterList.Count; i++) {
            ThrusterGenome genomeCopy = new ThrusterGenome(templateGenome.thrusterList[i]);
            thrusterList.Add(genomeCopy);
        }
        torqueList = new List<TorqueGenome>();
        for (int i = 0; i < templateGenome.torqueList.Count; i++) {
            TorqueGenome genomeCopy = new TorqueGenome(templateGenome.torqueList[i]);
            torqueList.Add(genomeCopy);
        }
        valueInputList = new List<ValueInputGenome>();
        for (int i = 0; i < templateGenome.valueInputList.Count; i++) {
            ValueInputGenome genomeCopy = new ValueInputGenome(templateGenome.valueInputList[i]);
            valueInputList.Add(genomeCopy);
        }
        weaponProjectileList = new List<WeaponProjectileGenome>();
        for (int i = 0; i < templateGenome.weaponProjectileList.Count; i++) {
            WeaponProjectileGenome genomeCopy = new WeaponProjectileGenome(templateGenome.weaponProjectileList[i]);
            weaponProjectileList.Add(genomeCopy);
        }
        weaponTazerList = new List<WeaponTazerGenome>();
        for (int i = 0; i < templateGenome.weaponTazerList.Count; i++) {
            WeaponTazerGenome genomeCopy = new WeaponTazerGenome(templateGenome.weaponTazerList[i]);
            weaponTazerList.Add(genomeCopy);
        }
    }

    public int GetCurrentHighestInnoValue() {
        int highestInno = -1;

        for (int i = 0; i < basicJointList.Count; i++) {
            if (basicJointList[i].inno > highestInno)
                highestInno = basicJointList[i].inno;
        }
        for (int i = 0; i < basicWheelList.Count; i++) {
            if (basicWheelList[i].inno > highestInno)
                highestInno = basicWheelList[i].inno;
        }
        for (int i = 0; i < contactSensorList.Count; i++) {
            if (contactSensorList[i].inno > highestInno)
                highestInno = contactSensorList[i].inno;
        }
        for (int i = 0; i < healthModuleList.Count; i++) {
            if (healthModuleList[i].inno > highestInno)
                highestInno = healthModuleList[i].inno;
        }
        for (int i = 0; i < oscillatorInputList.Count; i++) {
            if (oscillatorInputList[i].inno > highestInno)
                highestInno = oscillatorInputList[i].inno;
        }
        for (int i = 0; i < raycastSensorList.Count; i++) {
            if (raycastSensorList[i].inno > highestInno)
                highestInno = raycastSensorList[i].inno;
        }
        for (int i = 0; i < targetSensorList.Count; i++) {
            if (targetSensorList[i].inno > highestInno)
                highestInno = targetSensorList[i].inno;
        }
        for (int i = 0; i < thrusterList.Count; i++) {
            if (thrusterList[i].inno > highestInno)
                highestInno = thrusterList[i].inno;
        }
        for (int i = 0; i < torqueList.Count; i++) {
            if (torqueList[i].inno > highestInno)
                highestInno = torqueList[i].inno;
        }
        for (int i = 0; i < valueInputList.Count; i++) {
            if (valueInputList[i].inno > highestInno)
                highestInno = valueInputList[i].inno;
        }
        for (int i = 0; i < weaponProjectileList.Count; i++) {
            if (weaponProjectileList[i].inno > highestInno)
                highestInno = weaponProjectileList[i].inno;
        }
        for (int i = 0; i < weaponTazerList.Count; i++) {
            if (weaponTazerList[i].inno > highestInno)
                highestInno = weaponTazerList[i].inno;
        }

        return highestInno;
    }
}

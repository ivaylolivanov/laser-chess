using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New UnitStats", menuName = "UnitStats", order = 1)]
public class UnitStats : ScriptableObject {
    public int maxHealth;
    public int attackPower;
    [Range(1, Utils.MAX_STEPS)]public int stepsLimit;
    public float intervalBetweenSteps;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Stats", menuName = "Stats", order = 1)]
public class Stats : ScriptableObject {
    public int maxHealth;
    public int attackPower;
    [Range(1, Utils.MAX_STEPS)] public int stepsLimit;
    [Range(1, Utils.MAX_STEPS)] public int attackLimit;
    public float intervalBetweenSteps;
}

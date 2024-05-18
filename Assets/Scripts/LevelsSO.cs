using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSO", menuName = "Create LevelsSO")]
public class LevelsSO : ScriptableObject {
    public static LevelsSO instance;

    public int number;
    public int width;
    public int height;
    public int moveCount;
    public List<ItemsSO> itemList;

    public static LevelsSO Instance {
        get {
            if (instance == null) {
                instance = Resources.Load<LevelsSO>("LevelsSO");
                if (instance == null) {
                    Debug.LogError("LevelsSO asset not found in resources.");
                }
            }
            return instance;
        }
    }
}

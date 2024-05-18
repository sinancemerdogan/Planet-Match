using UnityEngine;

public class PersistentLevel : MonoBehaviour {
    public LevelsSO levelsSOAsset;

    //Singleton pattern for level information
    private void Awake() {
        if (LevelsSO.instance == null) {
            LevelsSO.instance = levelsSOAsset;
            //Does not destroy to object to make it accesible from LevelScene
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
}

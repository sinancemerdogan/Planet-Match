using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelValue;
    [SerializeField] private ItemsSO[] itemPrefabs;
    private LevelPopUpManager levelPopUpManager; 
    private SceneTransition sceneTransition;
    


    private void Awake() {
        levelPopUpManager = FindObjectOfType<LevelPopUpManager>();
        sceneTransition = FindObjectOfType<SceneTransition>();
    }

    //On Click of the Play Button sets the level information and loads the Level Scene
    public void OnClick() {

        Dictionary<string, string> levelDictionary = levelPopUpManager.GetlevelFilesDictionaryByKey(int.Parse(levelValue.text));

        LevelsSO levelSO = LevelsSO.Instance;

        levelSO.number = int.Parse(levelDictionary.GetValueOrDefault("level_number", "0"));
        levelSO.width = int.Parse(levelDictionary.GetValueOrDefault("grid_width", "0"));
        levelSO.height = int.Parse(levelDictionary.GetValueOrDefault("grid_height", "0"));
        levelSO.moveCount = int.Parse(levelDictionary.GetValueOrDefault("move_count", "0"));

        string[] itemArray = levelDictionary.GetValueOrDefault("grid", "").Split(",");

        levelSO.itemList.Clear();

        foreach (string item in itemArray) {

            switch (item) {
                case "r":
                    levelSO.itemList.Add(itemPrefabs[0]);
                    break;
                case "g":
                    levelSO.itemList.Add(itemPrefabs[1]);
                    break;
                case "b":
                    levelSO.itemList.Add(itemPrefabs[2]);
                    break;
                case "y":
                    levelSO.itemList.Add(itemPrefabs[3]);
                    break;
                default:
                    levelSO.itemList.Add(itemPrefabs[0]);
                    break;
            }
        }
        sceneTransition.LoadScene();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject levelsPopUp;
    [SerializeField] private Animator animator;
    private CelebrationCanvas celebrationCanvas;

    private void Awake() {

        //Automatically shows the Levels Pop Up after a level completion
        if (PlayerPrefs.GetInt("LevelsPopUpActivated") == 1) {
            animator.SetTrigger("Open LevelsPopUp");
        }
        celebrationCanvas = FindObjectOfType<CelebrationCanvas>();
        celebrationCanvas.transform.gameObject.SetActive(false);
    }
    private void Start() {

        //If level completed with a new high score, shows the celebration canvas
        if (PlayerPrefs.HasKey("Highest Score") && PlayerPrefs.HasKey("Current Level")) {
            if (PlayerPrefs.GetInt("Highest Score") == 1) {
                celebrationCanvas.transform.gameObject.SetActive(true);
                celebrationCanvas.OnGameOverWithNewHighScore();
            }
        }
    }
    private void OnApplicationQuit() {
        PlayerPrefs.SetInt("LevelsPopUpActivated", 0);
    }
}

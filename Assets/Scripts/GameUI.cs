using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using System;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{

    private TextMeshProUGUI moveCountText;
    private TextMeshProUGUI scoreText;
    [SerializeField] private GameManager gameGrid;
    private SceneTransition sceneTransition;
    

    private void Awake() {

        scoreText = transform.Find("Score Value").GetComponent<TextMeshProUGUI>();
        moveCountText = transform.Find("Move Count Value").GetComponent<TextMeshProUGUI>();
        gameGrid.OnGameOver += GameGrid_OnGameOver;
        gameGrid.OnGameOverWithNewHighScore += GameGrid_OnGameOverWithNewHighScore;
        sceneTransition = FindObjectOfType<SceneTransition>();

    }
    void Update()
    {
        UpdateTextFields();
    }

    //Updates text fields in the game UI
    private void UpdateTextFields() {
        scoreText.text = gameGrid.GetScore().ToString();
        moveCountText.text = gameGrid.GetMoveCount().ToString();
    }

    //On game over sets the LevelsPopUp to "activated", Highest Score to "false", and loads MainScene
    private void GameGrid_OnGameOver(object sender, System.EventArgs e) {
        PlayerPrefs.SetInt("LevelsPopUpActivated", 1);
        PlayerPrefs.SetInt("Highest Score", 0);
        sceneTransition.LoadScene();
    }
    //On game over with new high socre sets the LevelsPopUp to "activated", Highest Score to "true", and loads MainScene
    private void GameGrid_OnGameOverWithNewHighScore(object sender, EventArgs e) {
        PlayerPrefs.SetInt("LevelsPopUpActivated", 1);
        PlayerPrefs.SetInt("Current Level", gameGrid.GetLevelNumber());
        PlayerPrefs.SetInt("Highest Score", 1);
        sceneTransition.LoadScene();
    }
}

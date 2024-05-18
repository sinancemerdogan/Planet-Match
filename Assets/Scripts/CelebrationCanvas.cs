using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CelebrationCanvas : MonoBehaviour {

    private TextMeshProUGUI highestScoreText;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource celebrationSound;
    [SerializeField] private AudioSource mainSceneMusic;

    //On game over with new high score adjust canvas and shows it for a duration
    public void OnGameOverWithNewHighScore() {
        mainSceneMusic.Stop();
        PlayerPrefs.SetInt("Highest Score", 0);
        highestScoreText = transform.Find("Highest Score Value").GetComponent<TextMeshProUGUI>();
        string levelScoreKey = "Level " + PlayerPrefs.GetInt("Current Level") + " Score";
        highestScoreText.text = PlayerPrefs.GetInt(levelScoreKey).ToString();
        animator.SetTrigger("New High Score");
        celebrationSound.Play();
        StartCoroutine(ShowCelebrationScreenCoroutine(6f));
    }

    private IEnumerator ShowCelebrationScreenCoroutine(float duration) {
        yield return new WaitForSeconds(duration);
        transform.gameObject.SetActive(false);
        mainSceneMusic.Play();
    }

}

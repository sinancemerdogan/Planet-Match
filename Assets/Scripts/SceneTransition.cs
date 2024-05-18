using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;

    //If in MainScene load LevelScene, load MainScene otherwise
    public void LoadScene() {
        StartCoroutine(LoadSeceneCoroutine((SceneManager.GetActiveScene().buildIndex + 1) % 2));
    }

    //Start animation, wait for animation to complete, load next scene
    IEnumerator LoadSeceneCoroutine(int index) {

        animator.SetTrigger("Start Loading");

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(index);
    }
}

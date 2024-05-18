using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameManager;

public class Item : MonoBehaviour {

    public GameManager gameManager;
    private SFXManager sfxManager;
    private ItemsSO itemSO;
    public int x;
    public int y;
    public bool isReadyToTick;
    private int score;

    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private const float swipeThreshold = 20f;

    private void Start() {
        sfxManager = FindObjectOfType<SFXManager>();
    }
    public void SetItem(int x, int y, GameManager gameGrid, ItemsSO itemSO, int itemScore) {
        this.x = x;
        this.y = y;
        this.gameManager = gameGrid;
        this.itemSO = itemSO;
        this.score = itemScore;
    }

    public ItemsSO GetItemSO() {
        return itemSO;
    }

    public void SetItemXY(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void Destroy() {
        isReadyToTick = true;
    }

    private void OnMouseDown() {
        gameManager.SelectAnItem(this);
        swipeStartPosition = Input.mousePosition;
    }

    private void OnMouseUp() {
        swipeEndPosition = Input.mousePosition;
        CheckSwipeGesture();

    }
    
    //Check swipe and determines swipe direction
    private void CheckSwipeGesture() {
        Vector2 swipeDirection = swipeEndPosition - swipeStartPosition;
        float swipeMagnitude = swipeDirection.magnitude;

        if (swipeMagnitude >= swipeThreshold && gameManager.gameState == GameStates.Ready) {
            gameManager.gameState = GameStates.Busy;
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y)) {
                if (swipeDirection.x > 0) {
                    // Swipe right
                    gameManager.TrySwapItem(MoveDirection.Right);
                }
                else {
                    // Swipe left
                    gameManager.TrySwapItem(MoveDirection.Left);
                }
            }
            else {
                if (swipeDirection.y > 0) {
                    // Swipe up
                    gameManager.TrySwapItem(MoveDirection.Up);
                }
                else {
                    // Swipe down
                    gameManager.TrySwapItem(MoveDirection.Down);
                }
            }
        }
    }
    public void SwapToPosition(int x, int y) {

        if (this != null) {
            StartCoroutine(SwapToPositionCoroutine(x, y));
        }
    }

    //Swaps an item to position (x,y) on the game grid
    public IEnumerator SwapToPositionCoroutine(int x, int y) {

        this.x = x;
        this.y = y;

        Vector2 currentPosition = transform.position;
        Vector2 nextPosition = new Vector2(x, y);

        float duration = 0.5f;
        float elapsedTime = 0.0f;

        sfxManager.PlayPlanetSwap();
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(currentPosition, nextPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = nextPosition;
    }

    public int GetScore() {
        return score;
    }
}
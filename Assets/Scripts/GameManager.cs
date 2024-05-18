using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
using System.Drawing;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject gridElementPrefab;
    [SerializeField] private ItemsSO tickItem;
    [SerializeField] private GameObject background;

    private SFXManager sfxManager;
    private LevelsSO levelsSO;
    private Grid grid;
    private int gridWidth;
    private int gridHeight;
    private int score;
    private int moveCount;
    private int itemSpeed;
    private Item[,] itemsOnGameGrid;
    private List<Item> toBeTicktedItemsList;
    private Item selectedItem;
    private Vector2Int screenSize;

    public enum GameStates { Busy, Ready, GameOver }
    public GameStates gameState = GameStates.Ready;

    public event EventHandler OnGameOver;
    public event EventHandler OnGameOverWithNewHighScore;

    private void Awake() {
        this.levelsSO = LevelsSO.instance;
        sfxManager = FindObjectOfType<SFXManager>();

    }
    private void Start() {
        Setup();
    }

    //Sets up the choosen level, game grid, and camera
    public void Setup() {

        moveCount = levelsSO.moveCount;
        gridWidth = levelsSO.width;
        gridHeight = levelsSO.height;
        grid = new Grid(gridWidth, gridHeight);
        itemsOnGameGrid = new Item[gridWidth, gridHeight];
        toBeTicktedItemsList = new List<Item>();
        screenSize = new Vector2Int(Screen.width, Screen.height);

        float offsetX = (gridWidth - 1) / 2f;
        float offsetY = (gridHeight - 1) / 2f;

        float aspectRatio = (float)Screen.width / Screen.height;
        float targetAspectRatio_16_9 = 9f / 16f;
        float targetAspectRatio_4_3 = 4f / 3f;
        float tolerance = 0.01f;

        if (Mathf.Abs(aspectRatio - targetAspectRatio_16_9) < tolerance) {

            float gridWidthWithUI = gridWidth;
            if ((float)gridHeight / (float)gridWidth >= 16f / 9f) {
                gridWidthWithUI = gridWidth + 1;
            }

            Camera.main.orthographicSize = gridWidthWithUI;
            Camera.main.transform.position = new Vector3(offsetX, offsetY + 0.5f, Camera.main.transform.position.z);
            background.transform.position = new Vector3(offsetX, offsetY, background.transform.position.z);
        }
        else if (Mathf.Abs(aspectRatio - targetAspectRatio_4_3) < tolerance) {
            float targetOrthographicSize = (gridHeight + 1) * 0.5f; // Consider additional UI panel height
            float cameraWidth = targetOrthographicSize * (4f / 3f);

            Camera.main.orthographicSize = targetOrthographicSize;
            Camera.main.transform.position = new Vector3(offsetX, offsetY + 0.5f, Camera.main.transform.position.z);
            background.transform.position = new Vector3(offsetX, offsetY, background.transform.position.z);
        }
        else {
            Debug.LogWarning("Unsupported aspect ratio.");
        }

        for (int i = 0; i < gridHeight; i++) {
            for (int j = 0; j < gridWidth; j++) {
                Vector2 position = new Vector2(j, i);
                int itemIndex = j + i * gridWidth;

                GameObject gridElement = InstantiateGridElement(gridElementPrefab, position, j, i);
                InstantiateItem(gridElementPrefab, position, itemIndex, gridElement, j, i);
            }
        }
    }

    //On screen size change calls OnScreenSizeChange
    void Update() {
        Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (currentScreenSize != screenSize) {
            screenSize = currentScreenSize;
            OnScreenSizeChange();
        }
    }

    //Updates the camera 
    void OnScreenSizeChange() {
        float offsetX = (gridWidth - 1) / 2f;
        float offsetY = (gridHeight - 1) / 2f;

        float aspectRatio = (float)Screen.width / Screen.height;
        float targetAspectRatio_16_9 = 9f / 16f;
        float targetAspectRatio_4_3 = 4f / 3f;
        float tolerance = 0.01f;

        if (Mathf.Abs(aspectRatio - targetAspectRatio_16_9) < tolerance) {

            float gridWidthWithUI = gridWidth;
            if ((float)gridHeight / (float)gridWidth >= 16f / 9f) {
                gridWidthWithUI = gridWidth + 1;
            }

            Camera.main.orthographicSize = gridWidthWithUI;
            Camera.main.transform.position = new Vector3(offsetX, offsetY + 0.5f, Camera.main.transform.position.z);
            background.transform.position = new Vector3(offsetX, offsetY, background.transform.position.z);
        }
        else if (Mathf.Abs(aspectRatio - targetAspectRatio_4_3) < tolerance) {
            float targetOrthographicSize = (gridHeight + 1) * 0.5f; // Consider additional UI panel height
            float cameraWidth = targetOrthographicSize * (4f / 3f);

            Camera.main.orthographicSize = targetOrthographicSize;
            Camera.main.transform.position = new Vector3(offsetX, offsetY + 0.5f, Camera.main.transform.position.z);
            background.transform.position = new Vector3(offsetX, offsetY, background.transform.position.z);
        }
        else {
            Debug.LogWarning("Unsupported aspect ratio.");
        }
    }

    //Instantiates a grid element on the grid
    private GameObject InstantiateGridElement(GameObject gridElementPrefab, Vector2 position, int i, int j) {

        GameObject gridElement = Instantiate(gridElementPrefab, position, Quaternion.identity);
        gridElement.transform.parent = this.transform;
        gridElement.name = "Grid Element(" + i + "," + j + ")";
        return gridElement;
    }
    //Instantiates a item on the grid
    private void InstantiateItem(GameObject gridElementPrefab, Vector2 position, int itemIndex, GameObject gridElement, int i, int j ) {

        
        Item item = Instantiate(levelsSO.itemList[itemIndex].prefab, new Vector2(position.x, position.y), Quaternion.identity);
        item.transform.parent = gridElement.transform;
        item.name = "Item(" + i + "," + j + ")";
        itemsOnGameGrid[i, j] = item;

        int itemScore = 0;
        if (levelsSO.itemList[itemIndex].itemName == "Red Item") {
            itemScore = 100;
        }
        else if (levelsSO.itemList[itemIndex].itemName == "Blue Item") {
            itemScore = 200;
        }
        else if (levelsSO.itemList[itemIndex].itemName == "Green Item") {
            itemScore = 150;
        }
        else if (levelsSO.itemList[itemIndex].itemName == "Yellow Item") {
            itemScore = 250;
        }
        item.SetItem((int)position.x, (int)position.y,this, levelsSO.itemList[itemIndex], itemScore);
    }

    //Finds all row matches on the grid
    public void FindAllRowMatches() {
        for (int j = 0; j < gridHeight; j++) {
            bool isFullRowMatch = true;
            Item firstItemInRow = null;

            for (int i = 0; i < gridWidth; i++) {
                Item currentItem = itemsOnGameGrid[i, j];

                if (currentItem.GetItemSO().itemName == "Tick Item") {
                    isFullRowMatch = false;
                    break;
                }

                if (firstItemInRow == null) {
                    firstItemInRow = currentItem;
                }
                else if (currentItem.GetItemSO().name != firstItemInRow.GetItemSO().name) {
                    isFullRowMatch = false;
                    break;
                }
            }

            if (isFullRowMatch) {
                for (int i = 0; i < gridWidth; i++) {
                    Item itemInRow = itemsOnGameGrid[i, j];
                    itemInRow.Destroy();
                    if (!toBeTicktedItemsList.Contains(itemInRow)) {
                        toBeTicktedItemsList.Add(itemInRow);
                    }
                }
            }
        }
    }

    //Changes matched items with the tick item
    public void TickMathches() {

        foreach (Item item in toBeTicktedItemsList) {
            TickMathchedItemAt(item.x, item.y);
        }
        toBeTicktedItemsList.Clear();
    }

    //Changes matched item at (x,y) with the tick item and instantiate particle effect for explosion
    public void TickMathchedItemAt(int x, int y) {


        if (itemsOnGameGrid[x, y] != null && itemsOnGameGrid[x, y].isReadyToTick) {
            Item itemToTick = itemsOnGameGrid[x, y];
            Transform parent = itemsOnGameGrid[x, y].transform.parent;
            Destroy(itemToTick.gameObject);
            GameObject explosionEffect =  Instantiate(itemToTick.GetItemSO().explosionEffect, new Vector2(x, y), Quaternion.identity);
            sfxManager.PlayPlanetExplosion();
            float explosionDuration = 1.5f; 
            StartCoroutine(WaitAndAddTickItem(x, y, explosionDuration, explosionEffect, parent));
        }
    }
    //Changes matched item at (x,y) with the tick item after explosion particle effect  
    private IEnumerator WaitAndAddTickItem(int x, int y, float duration, GameObject explosionEffect, Transform parent) {

        score += itemsOnGameGrid[x, y].GetScore();
        yield return new WaitForSeconds(duration);

        Destroy(explosionEffect);
        Item item = Instantiate(tickItem.prefab, new Vector2(x, y), Quaternion.identity);
        item.transform.parent = parent;
        item.name = "Tick Item(" + x + "," + y + ")";
        itemsOnGameGrid[x, y] = item;
        item.SetItem(x, y, this, tickItem, 0);
        IsGameOver();
    }

    //Checks if the game is over and invokes the game over function accordingly
    public void IsGameOver() {
        if ((moveCount <= 0 || FindIslands()) && gameState != GameStates.GameOver) {
            string levelScoreKey = "Level " + levelsSO.number + " Score";

            if (PlayerPrefs.HasKey(levelScoreKey)) {
                if (score > PlayerPrefs.GetInt(levelScoreKey)) {
                    PlayerPrefs.SetInt(levelScoreKey, score);
                    gameState = GameStates.GameOver;
                    StartCoroutine(InvokeEventWithDelay(OnGameOverWithNewHighScore));
                }
                else {
                    gameState = GameStates.GameOver;
                    StartCoroutine(InvokeEventWithDelay(OnGameOver));
                }
            }
            else {
                PlayerPrefs.SetInt(levelScoreKey, score);
                gameState = GameStates.GameOver;
                if (score != 0) {
                    StartCoroutine(InvokeEventWithDelay(OnGameOverWithNewHighScore));
                }
                else {
                    StartCoroutine(InvokeEventWithDelay(OnGameOver));
                }
            }
        }
        else if(gameState != GameStates.GameOver) {
            gameState = GameStates.Ready;
        }
    }

    private IEnumerator InvokeEventWithDelay(EventHandler eventHandler) {
        yield return new WaitForSeconds(1f);
        eventHandler?.Invoke(this, EventArgs.Empty);
    }

    //Swap to items according to direction
    public void SwapItems(MoveDirection direction) {

        int nextX = selectedItem.x;
        int nextY = selectedItem.y;

        int tempX = selectedItem.x;
        int tempY = selectedItem.y;

        if (direction == MoveDirection.Right) {
            nextX += 1;
        }
        else if (direction == MoveDirection.Left) {
            nextX -= 1;
        }
        else if (direction == MoveDirection.Up) {
            nextY += 1;
        }
        else if (direction == MoveDirection.Down) {
            nextY -= 1;
        }

        if (IsSwapValid(nextX, nextY)) {
            Item nextItem = itemsOnGameGrid[nextX, nextY];

            if(IsItemSwappable(nextItem)) {
                nextItem.SetItemXY(selectedItem.x, selectedItem.y);
                selectedItem.SetItemXY(nextX, nextY);

                itemsOnGameGrid[selectedItem.x, selectedItem.y] = selectedItem;
                itemsOnGameGrid[nextItem.x, nextItem.y] = nextItem;

                selectedItem.SwapToPosition(nextX, nextY);
                nextItem.SwapToPosition(tempX, tempY);

                DecreaseMoveCount();
                StartCoroutine(CheckGrid());
            }
        }
        
    }

    //Checks if a swap operation is valid
    private bool IsSwapValid(int x, int y) {

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) 
            return false;
        else 
            return true;
    }

    //Checks if the item is swappable
    private bool IsItemSwappable(Item item) {
        bool swappable = item.GetItemSO().itemName != "Tick Item" && item != null;

        if(swappable) {
            return true;
        }
        else {
            gameState = GameStates.Ready;
            return false;
        }
    }
    //Selects an item on the grid
    public void SelectAnItem(Item item) {
        selectedItem = item;
    }

    //Tries to swap item according to direction
    public void TrySwapItem(MoveDirection direction) {
            SwapItems(direction);
    }
    public IEnumerator CheckGrid() {

        yield return new WaitForSeconds(.5f);

        IsGameOver();
        FindAllRowMatches();
        TickMathches();
    }
    public bool FindIslands() {
        bool[,] visited = new bool[gridWidth, gridHeight];
        List<List<Item>> islands = new List<List<Item>>();
        List<Item> currentIsland = null;

        for (int j = 0; j < gridHeight; j++) {
            if (IsSeparatorRow(j)) {
                // Add a new island when encountering a separator row
                currentIsland = new List<Item>();
                islands.Add(currentIsland);
            }
            else {
                for (int i = 0; i < gridWidth; i++) {
                    if (!visited[i, j] && itemsOnGameGrid[i, j] != null && !IsSeparatorItem(itemsOnGameGrid[i, j])) {
                        if (currentIsland == null) {
                            currentIsland = new List<Item>();
                            islands.Add(currentIsland);
                        }
                        currentIsland.Add(itemsOnGameGrid[i, j]);
                        ExploreIsland(i, j, visited, currentIsland);
                    }
                }
                currentIsland = null; // Reset the current island after processing the row
            }
        }

        foreach (List<Item> island in islands) {

            int redItemCount = 0;
            int greenItemCount = 0;
            int blueItemCount = 0;
            int yellowItemCount = 0;

            foreach (Item item in island) {

                if (item.GetItemSO().itemName == "Red Item") {
                    redItemCount += 1;
                }
                else if (item.GetItemSO().itemName == "Green Item") {
                    greenItemCount += 1;
                }
                else if (item.GetItemSO().itemName == "Blue Item") {
                    blueItemCount += 1;
                }
                else if (item.GetItemSO().itemName == "Yellow Item") {
                    yellowItemCount += 1;
                }
            }
            if (redItemCount >= gridWidth || greenItemCount >= gridWidth || blueItemCount >= gridWidth || yellowItemCount >= gridWidth) {
                return false;
            }
        }
        return true;
    }

    private bool IsSeparatorRow(int row) {
        return itemsOnGameGrid[0, row] != null && IsSeparatorItem(itemsOnGameGrid[0, row]);
    }

    private bool IsSeparatorItem(Item item) {
        return item.GetItemSO().itemName == "Tick Item";
    }

    private void ExploreIsland(int x, int y, bool[,] visited, List<Item> island) {
        visited[x, y] = true;

        // Check neighboring items (up, down, left, right)
        CheckNeighbor(x, y + 1, visited, island); // Up
        CheckNeighbor(x, y - 1, visited, island); // Down
        CheckNeighbor(x - 1, y, visited, island); // Left
        CheckNeighbor(x + 1, y, visited, island); // Right
    }

    private void CheckNeighbor(int x, int y, bool[,] visited, List<Item> island) {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight) {
            if (!visited[x, y] && itemsOnGameGrid[x, y] != null && itemsOnGameGrid[x, y].GetItemSO() != null && !IsSeparatorItem(itemsOnGameGrid[x, y])) {
                island.Add(itemsOnGameGrid[x, y]);
                ExploreIsland(x, y, visited, island);
            }
        }
    }


    private void DecreaseMoveCount() {
        moveCount -= 1;
    }

    public int GetGridWidth() {
        return this.gridWidth;
    }
    public int GetGridHeight() {
        return this.gridHeight;
    }
    public Item[,] GetItemsOnGameGrid() {
        return itemsOnGameGrid;
    }

    public void SetLevelSO(LevelsSO levelsSO) {
        this.levelsSO = levelsSO;
    }
    public int GetScore() {
        return this.score;
    }
    public int GetMoveCount() {
        return this.moveCount;
    }
    public void SetMoveCount(int moveCount) {
        this.moveCount = moveCount;
    }
    public int GetLevelNumber() {
        return levelsSO.number;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Grid
{

    private const int MAX_WIDTH = 9;
    private const int MAX_HEIGHT = 9;

    [SerializeField] public int width;
    [SerializeField] public int height;

    private GridElement[,] gridElements;

   public Grid(int width, int height) {
        this.width = width;
        this.height = height;

        gridElements = new GridElement[width,height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                gridElements[x, y] = new GridElement(this, x, y);
            }
        }
    }
    public int GetWidth() {
        return width;
    }
    public int GetHeight() {
        return height;
    }
    public GridElement GetGridElement(int x, int y) {
        return gridElements[x, y];
    }

    //public void SetGridElement(int x, int y) {
    //    return gridElements[x, y];
    //}
}

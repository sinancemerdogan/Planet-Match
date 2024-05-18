using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridElement
{
    private Item itemOnGrid;
    private Grid grid;
    private int x;
    private int y;

    public GridElement(Grid grid, int x, int y) {

        this.grid = grid;
        this.x = x;
        this.y = y;

    }

    public int GetX() {
        return x;
    }

    public int GetY() {
        return y;
    }

    /*public Vector3 GetWorldPosition() {
        return grid.GetWorldPosition(x, y);
    }*/

    public Item GetItemOnGrid() {
        return itemOnGrid;
    }
    public void SetItemOnGrid(Item itemOnGrid) {
        this.itemOnGrid = itemOnGrid;
        //grid.TriggerGridObjectChanged(x, y);
    }

    public void ClearItemOnGrid() {
        itemOnGrid = null;
    }

    public void DestroyGem() {
        //itemOnGrid?.Destroy();
        //grid.TriggerGridObjectChanged(x, y);
    }

    public bool HasGemGrid() {
        return itemOnGrid != null;
    }

    public bool IsEmpty() {
        return itemOnGrid == null;
    }

}

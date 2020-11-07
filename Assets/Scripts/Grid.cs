using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    [Header("Info")]
    private Vector2Int gridDimension;

    private int[,] grid;

    private Vector2 topLeft;

    private Vector2 bottomRight;

    public Grid(Vector2Int dimension, Vector2 topLeft)
    {
        this.topLeft = topLeft;
        CreateGrid(dimension);
        bottomRight = new Vector2(topLeft.x + dimension.x, topLeft.y + dimension.y);

        Debug.Log(bottomRight);
        Debug.Log(topLeft);
    }


    // [row][column] -> [y][x]

    public void CreateGrid(Vector2Int dimension)
    {
        this.gridDimension = dimension;
        this.grid = new int[dimension.y, dimension.x];
    }

    public Vector2 GetNearestPos(Vector3 position)
    {
        Vector2 pos = new Vector2(position.x, position.y);
        float y = Mathf.Abs((pos.y - topLeft.y) / (bottomRight.y - topLeft.y));
        float x = Mathf.Abs((pos.x - topLeft.x) / (bottomRight.x - topLeft.x));
        return new Vector2(x * gridDimension.x, y * gridDimension.y);

        return Vector2Int.zero;
      // return new Vector2Int((int) x * gridDimension, (int) y * gridDimension);

       // NewValue = (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin
    }


    public Vector3 GetPosition(Vector2Int pos, Vector2 offset)
    {
        return new Vector3(topLeft.x + pos.x, topLeft.y - pos.y, 0);
    }


    public int AtPos(Vector2Int pos)
    {
        return grid[pos.y, pos.x];
    }

    public void SetPos(Vector2Int pos, int value)
    {
        grid[pos.y, pos.x] = value;
    }


    public void ClearGrid()
    {
        for (int row = 0; row < gridDimension.y; row++)
        {
            for (int col = 0; col < gridDimension.x; col++)
            {
                grid[row, col] = 0;

            }
        }
    }

    public void MoveItem(Vector2Int oldPos, Vector2Int newPos)
    {
        grid[newPos.y, newPos.x] = grid[oldPos.y, oldPos.x];
        grid[oldPos.y, oldPos.x] = 0;
    }




    public bool ValidPos(Vector2Int pos)
    {
        if ((pos.x >= 0 && pos.x < gridDimension.x) &&
            (pos.y >= 0 && pos.y < gridDimension.y))
        {
            return true;
        }
        return false;
    }

    //public List<Vector2Int> Adjacent(Vector2Int pos)
    //{
    //    List<Vector2Int> storage = new List<Vector2Int>();
    //    Vector2Int left = LoopPosition(new Vector2Int(pos.x - 1, pos.y));
    //    Vector2Int right = LoopPosition(new Vector2Int(pos.x + 1, pos.y));
    //    Vector2Int top = LoopPosition(new Vector2Int(pos.x, pos.y + 1));
    //    Vector2Int bottom = LoopPosition(new Vector2Int(pos.x, pos.y - 1));
    //    storage.Add(left);
    //    storage.Add(right);
    //    storage.Add(top);
    //    storage.Add(bottom);
    //    return storage;
    //}


    //private Vector2Int LoopPosition(Vector2Int inPos)
    //{
    //    Vector2Int temp = inPos;
    //    if (temp.x < 0) temp.x = gridDimension - 1;
    //    else if (temp.x >= gridDimension) temp.x = 0;
    //    if (temp.y < 0) temp.y = gridDimension - 1;
    //    else if (temp.y >= gridDimension) temp.y = 0;
    //    return temp;
    //}

    public Vector2Int RandPos(Vector2Int offset)
    {
        return new Vector2Int(Random.Range(0 + offset.x, gridDimension.x - offset.x), Random.Range(0 + offset.y, gridDimension.y - offset.y));
    }
}

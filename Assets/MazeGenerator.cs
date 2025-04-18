using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth = 10;

    [SerializeField]
    private int _mazeDepth = 10;

    [SerializeField]
    private int _mazeHeight = 10;

    private MazeCell[,,] _mazeGrid;

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int y = 0; y < _mazeHeight; y++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    _mazeGrid[x, y, z] = Instantiate(_mazeCellPrefab, new Vector3(x, y, z), Quaternion.identity);
                }
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0, 0]);

        ActivateFurthestEndBlock();
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int y = (int)currentCell.transform.position.y;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, y, z];
            if (!cellToRight.IsVisited) yield return cellToRight;
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, y, z];
            if (!cellToLeft.IsVisited) yield return cellToLeft;
        }

        if (y + 1 < _mazeHeight)
        {
            var cellAbove = _mazeGrid[x, y + 1, z];
            if (!cellAbove.IsVisited) yield return cellAbove;
        }

        if (y - 1 >= 0)
        {
            var cellBelow = _mazeGrid[x, y - 1, z];
            if (!cellBelow.IsVisited) yield return cellBelow;
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, y, z + 1];
            if (!cellToFront.IsVisited) yield return cellToFront;
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, y, z - 1];
            if (!cellToBack.IsVisited) yield return cellToBack;
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
        }
        else if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
        }
        else if (previousCell.transform.position.y < currentCell.transform.position.y)
        {
            previousCell.ClearTopWall();
            currentCell.ClearDownWall();
        }
        else if (previousCell.transform.position.y > currentCell.transform.position.y)
        {
            previousCell.ClearDownWall();
            currentCell.ClearTopWall();
        }
        else if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
        }
        else if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
        }
    }

    private void ActivateFurthestEndBlock()
    {
        var visitedCells = new List<MazeCell>();
        var stack = new Stack<MazeCell>();
        var startCell = _mazeGrid[0, 0, 0];
        stack.Push(startCell);

        MazeCell furthestCell = startCell;

        while (stack.Count > 0)
        {
            var currentCell = stack.Pop();

            if (!visitedCells.Contains(currentCell))
            {
                visitedCells.Add(currentCell);
                furthestCell = currentCell;

                foreach (var neighbor in GetUnvisitedCells(currentCell))
                {
                    stack.Push(neighbor);
                }
            }
        }

        furthestCell.ActivateEndBlock();
    }
}
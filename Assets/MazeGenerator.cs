using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth = 30;

    [SerializeField]
    private int _mazeDepth = 30;

    [SerializeField]
    private int _mazeHeight = 30;

    private MazeCell[,,] _mazeGrid;

    void Start()
    {
        Debug.Log($"Initializing maze grid with dimensions: {_mazeWidth}x{_mazeHeight}x{_mazeDepth}");
        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int y = 0; y < _mazeHeight; y++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    _mazeGrid[x, y, z] = Instantiate(_mazeCellPrefab, new Vector3(x, y, z), Quaternion.identity);
                    Debug.Log($"Created MazeCell at position ({x}, {y}, {z})");
                }
            }
        }

        Debug.Log("Starting maze generation...");
        GenerateMaze(_mazeGrid[0, 0, 0]);
    }

    private void GenerateMaze(MazeCell startCell)
    {
        Debug.Log("Generating maze...");
        Stack<MazeCell> stack = new Stack<MazeCell>();
        stack.Push(startCell);

        MazeCell longestPathCell = startCell;
        int maxDepth = 0;

        Dictionary<MazeCell, int> cellDepths = new Dictionary<MazeCell, int>();
        cellDepths[startCell] = 0;

        while (stack.Count > 0)
        {
            MazeCell currentCell = stack.Peek();
            currentCell.Visit();
            Debug.Log($"Visiting cell at position {currentCell.transform.position}");

            MazeCell nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                Debug.Log($"Moving to next cell at position {nextCell.transform.position}");
                ClearWalls(currentCell, nextCell);
                stack.Push(nextCell);

                int currentDepth = cellDepths[currentCell] + 1;
                cellDepths[nextCell] = currentDepth;

                if (currentDepth > maxDepth)
                {
                    maxDepth = currentDepth;
                    longestPathCell = nextCell;
                    Debug.Log($"New longest path found with depth {maxDepth} at cell {longestPathCell.transform.position}");
                }
            }
            else
            {
                Debug.Log($"Backtracking from cell at position {currentCell.transform.position}");
                stack.Pop();
            }
        }

        Debug.Log($"Maze generation complete. Longest path ends at {longestPathCell.transform.position} with depth {maxDepth}");
        longestPathCell.ActivateEndBlock();
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        if (unvisitedCells.Count > 0)
        {
            Debug.Log($"Found {unvisitedCells.Count} unvisited neighbors for cell at position {currentCell.transform.position}");
            return unvisitedCells[Random.Range(0, unvisitedCells.Count)];
        }

        Debug.Log($"No unvisited neighbors for cell at position {currentCell.transform.position}");
        return null;
    }

    private List<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        List<MazeCell> unvisitedCells = new List<MazeCell>();

        int x = (int)currentCell.transform.position.x;
        int y = (int)currentCell.transform.position.y;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, y, z].IsVisited)
        {
            unvisitedCells.Add(_mazeGrid[x + 1, y, z]);
        }

        if (x - 1 >= 0 && !_mazeGrid[x - 1, y, z].IsVisited)
        {
            unvisitedCells.Add(_mazeGrid[x - 1, y, z]);
        }

        if (y + 1 < _mazeHeight && !_mazeGrid[x, y + 1, z].IsVisited)
        {
            unvisitedCells.Add(_mazeGrid[x, y + 1, z]);
        }

        if (y - 1 >= 0 && !_mazeGrid[x, y - 1, z].IsVisited)
        {
            unvisitedCells.Add(_mazeGrid[x, y - 1, z]);
        }

        if (z + 1 < _mazeDepth && !_mazeGrid[x, y, z + 1].IsVisited)
        {
            unvisitedCells.Add(_mazeGrid[x, y, z + 1]);
        }

        if (z - 1 >= 0 && !_mazeGrid[x, y, z - 1].IsVisited)
        {
            unvisitedCells.Add(_mazeGrid[x, y, z - 1]);
        }

        return unvisitedCells;
    }

    private void ClearWalls(MazeCell currentCell, MazeCell nextCell)
    {
        Vector3 direction = nextCell.transform.position - currentCell.transform.position;
        Debug.Log($"Clearing walls between cells at {currentCell.transform.position} and {nextCell.transform.position}");

        if (direction.x > 0)
        {
            currentCell.ClearRightWall();
            nextCell.ClearLeftWall();
        }
        else if (direction.x < 0)
        {
            currentCell.ClearLeftWall();
            nextCell.ClearRightWall();
        }
        else if (direction.y > 0)
        {
            currentCell.ClearTopWall();
            nextCell.ClearDownWall();
        }
        else if (direction.y < 0)
        {
            currentCell.ClearDownWall();
            nextCell.ClearTopWall();
        }
        else if (direction.z > 0)
        {
            currentCell.ClearFrontWall();
            nextCell.ClearBackWall();
        }
        else if (direction.z < 0)
        {
            currentCell.ClearBackWall();
            nextCell.ClearFrontWall();
        }
    }
}
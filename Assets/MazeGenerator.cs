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
        InitializeMazeGrid();
        GenerateMaze(_mazeGrid[0, 0, 0]);
    }

    private void InitializeMazeGrid()
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
                }
            }
        }
    }

    private void GenerateMaze(MazeCell startCell)
    {
        Debug.Log("Starting maze generation...");
        Stack<MazeCell> stack = new Stack<MazeCell>();
        stack.Push(startCell);

        MazeCell furthestCell = startCell;
        int maxDistance = 0;

        startCell.cellDist = 0;

        while (stack.Count > 0)
        {
            MazeCell currentCell = stack.Peek();
            currentCell.Visit();

            MazeCell nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                ClearWallsBetween(currentCell, nextCell);
                stack.Push(nextCell);

                nextCell.cellDist = currentCell.cellDist + 1;

                if (nextCell.cellDist > maxDistance)
                {
                    maxDistance = nextCell.cellDist;
                    furthestCell = nextCell;
                }
            }
            else
            {
                stack.Pop();
            }
        }

        Debug.Log($"Maze generation complete. Furthest cell is at {furthestCell.transform.position} with distance {maxDistance}");
        furthestCell.ActivateEndBlock();
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedNeighbors(currentCell);
        return unvisitedCells.Count > 0 ? unvisitedCells[Random.Range(0, unvisitedCells.Count)] : null;
    }

    private List<MazeCell> GetUnvisitedNeighbors(MazeCell currentCell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
        };

        Vector3Int currentPos = Vector3Int.RoundToInt(currentCell.transform.position);

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = currentPos + dir;
            if (IsWithinBounds(neighborPos) && !_mazeGrid[neighborPos.x, neighborPos.y, neighborPos.z].IsVisited)
            {
                neighbors.Add(_mazeGrid[neighborPos.x, neighborPos.y, neighborPos.z]);
            }
        }

        return neighbors;
    }

    private bool IsWithinBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < _mazeWidth &&
               pos.y >= 0 && pos.y < _mazeHeight &&
               pos.z >= 0 && pos.z < _mazeDepth;
    }

    private void ClearWallsBetween(MazeCell currentCell, MazeCell nextCell)
    {
        Vector3 direction = nextCell.transform.position - currentCell.transform.position;

        if (direction.x > 0) { currentCell.ClearRightWall(); nextCell.ClearLeftWall(); }
        else if (direction.x < 0) { currentCell.ClearLeftWall(); nextCell.ClearRightWall(); }
        else if (direction.y > 0) { currentCell.ClearTopWall(); nextCell.ClearDownWall(); }
        else if (direction.y < 0) { currentCell.ClearDownWall(); nextCell.ClearTopWall(); }
        else if (direction.z > 0) { currentCell.ClearFrontWall(); nextCell.ClearBackWall(); }
        else if (direction.z < 0) { currentCell.ClearBackWall(); nextCell.ClearFrontWall(); }
    }
}
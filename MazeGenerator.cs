using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private int _mazeHeight; // New height parameter

    private static float _mazeSizeMultiplier = 1.0f;

    private MazeCell[,,] _mazeGrid; // Updated to 3D array

    private void Awake()
    {
        _mazeSizeMultiplier = PlayerPrefs.GetFloat("MazeSizeMultiplier", 1.0f);
    }

    void Start()
    {
        _mazeSizeMultiplier = PlayerPrefs.GetFloat("MazeSizeMultiplier", 1.0f);

        _mazeWidth = Mathf.RoundToInt(_mazeWidth * _mazeSizeMultiplier);
        _mazeDepth = Mathf.RoundToInt(_mazeDepth * _mazeSizeMultiplier);
        _mazeHeight = Mathf.RoundToInt(_mazeHeight * _mazeSizeMultiplier);

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

        GenerateMazeDFS();
        PlaceFurthestEndBlock();
    }

    private void GenerateMazeDFS()
    {
        Stack<(MazeCell cell, MazeCell parent)> stack = new Stack<(MazeCell, MazeCell)>();
        MazeCell startCell = _mazeGrid[0, 0, 0];

        stack.Push((startCell, null));

        while (stack.Count > 0)
        {
            var (currentCell, previousCell) = stack.Pop();

            if (currentCell.IsVisited)
                continue;

            currentCell.Visit();
            ClearWalls(previousCell, currentCell);

            foreach (var neighbor in GetUnvisitedNeighbors(currentCell).OrderBy(_ => UnityEngine.Random.Range(0, 100)))
            {
                stack.Push((neighbor, currentCell));
            }
        }
    }

    private void PlaceFurthestEndBlock()
    {
        MazeCell startCell = _mazeGrid[0, 0, 0];
        MazeCell furthestCell = FindFurthestCell(startCell);
        furthestCell.ActivateEndBlock();
    }

    private MazeCell FindFurthestCell(MazeCell startCell)
    {
        Queue<(MazeCell cell, int distance)> queue = new Queue<(MazeCell, int)>();
        HashSet<MazeCell> visited = new HashSet<MazeCell>();
        MazeCell furthestCell = startCell;
        int maxDistance = 0;

        queue.Enqueue((startCell, 0));
        visited.Add(startCell);

        while (queue.Count > 0)
        {
            var (currentCell, distance) = queue.Dequeue();

            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestCell = currentCell;
            }

            foreach (var neighbor in GetVisitedNeighbors(currentCell))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue((neighbor, distance + 1));
                }
            }
        }

        return furthestCell;
    }

    private IEnumerable<MazeCell> GetUnvisitedNeighbors(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int y = (int)currentCell.transform.position.y;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, y, z].IsVisited)
            yield return _mazeGrid[x + 1, y, z];
        if (x - 1 >= 0 && !_mazeGrid[x - 1, y, z].IsVisited)
            yield return _mazeGrid[x - 1, y, z];
        if (z + 1 < _mazeDepth && !_mazeGrid[x, y, z + 1].IsVisited)
            yield return _mazeGrid[x, y, z + 1];
        if (z - 1 >= 0 && !_mazeGrid[x, y, z - 1].IsVisited)
            yield return _mazeGrid[x, y, z - 1];
        if (y + 1 < _mazeHeight && !_mazeGrid[x, y + 1, z].IsVisited) // Top neighbor
            yield return _mazeGrid[x, y + 1, z];
        if (y - 1 >= 0 && !_mazeGrid[x, y - 1, z].IsVisited) // Down neighbor
            yield return _mazeGrid[x, y - 1, z];
    }

    private IEnumerable<MazeCell> GetVisitedNeighbors(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int y = (int)currentCell.transform.position.y;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth && _mazeGrid[x + 1, y, z].IsVisited)
            yield return _mazeGrid[x + 1, y, z];
        if (x - 1 >= 0 && _mazeGrid[x - 1, y, z].IsVisited)
            yield return _mazeGrid[x - 1, y, z];
        if (z + 1 < _mazeDepth && _mazeGrid[x, y, z + 1].IsVisited)
            yield return _mazeGrid[x, y, z + 1];
        if (z - 1 >= 0 && _mazeGrid[x, y, z - 1].IsVisited)
            yield return _mazeGrid[x, y, z - 1];
        if (y + 1 < _mazeHeight && _mazeGrid[x, y + 1, z].IsVisited)
            yield return _mazeGrid[x, y + 1, z];
        if (y - 1 >= 0 && _mazeGrid[x, y - 1, z].IsVisited)
            yield return _mazeGrid[x, y - 1, z];
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        Vector3 diff = currentCell.transform.position - previousCell.transform.position;

        if (diff.x > 0)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
        }
        else if (diff.x < 0)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
        }
        else if (diff.z > 0)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
        }
        else if (diff.z < 0)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
        }
        else if (diff.y > 0)
        {
            previousCell.ClearTopWall();
            currentCell.ClearDownWall();
        }
        else if (diff.y < 0)
        {
            previousCell.ClearDownWall();
            currentCell.ClearTopWall();
        }
    }
}
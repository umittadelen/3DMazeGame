using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;

    [SerializeField]
    private GameObject _topWall;

    [SerializeField]
    private GameObject _downWall;

    [SerializeField]
    private GameObject _unvisitedBlock;

    [SerializeField]
    private GameObject _endBlock;

    public int cellDist = 0;

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
        _endBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }

    public void ClearTopWall()
    {
        _topWall.SetActive(false);
    }

    public void ClearDownWall()
    {
        _downWall.SetActive(false);
    }

    public void ActivateEndBlock()
    {
        _endBlock.SetActive(true);
    }
}

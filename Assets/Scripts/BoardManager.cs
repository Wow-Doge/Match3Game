using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameState
{
    Idling,
    Moving
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public GameState currentState = GameState.Idling;
    public int column;
    public int row;
    public int offset;
    public float awaitTime = 0.25f;

    public GameObject[,] candyPosition;

    public List<GameObject> candyType = new List<GameObject>();
    public List<GameObject> currentMatches = new List<GameObject>();

    public GameObject selectedCandy;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        candyPosition = new GameObject[row, column];
        CreateBoard();
    }

    public void CreateBoard()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offset);
                int random = Random.Range(0, candyType.Count);

                int maxIterations = 0;
                while (CheckMatchInit(i, j, candyType[random]) && maxIterations < 100)
                {
                    random = Random.Range(0, candyType.Count);
                    maxIterations++;
                }

                GameObject newCandy = Instantiate(candyType[random], tempPosition, Quaternion.identity);
                newCandy.GetComponent<Candy>().atRow = j;
                newCandy.GetComponent<Candy>().atColumn = i;
                newCandy.transform.parent = this.transform;
                newCandy.name = newCandy.name.Replace("(Clone)", "");
                candyPosition[i, j] = newCandy;
            }
        }
    }

    public void ScanBoard()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                GameObject currentCandy = candyPosition[i, j];
                if (currentCandy != null)
                {
                    if (i > 0 && i < row - 1)
                    {
                        GameObject leftCandy = candyPosition[i - 1, j];
                        GameObject rightCandy = candyPosition[i + 1, j];
                        if (leftCandy != null && rightCandy != null)
                        {
                            if (leftCandy.name == currentCandy.name && rightCandy.name == currentCandy.name)
                            {
                                currentCandy.GetComponent<Candy>().isMatched = true;
                                leftCandy.GetComponent<Candy>().isMatched = true;
                                rightCandy.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < column - 1)
                    {
                        GameObject downCandy = candyPosition[i, j - 1];
                        GameObject upCandy = candyPosition[i, j + 1];
                        if (downCandy != null && upCandy != null)
                        {
                            if (downCandy.name == currentCandy.name && upCandy.name == downCandy.name)
                            {
                                currentCandy.GetComponent<Candy>().isMatched = true;
                                upCandy.GetComponent<Candy>().isMatched = true;
                                downCandy.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
        AddToCurrentMatches();
        GetChain();
        DestroyCandy();
    }

    public void AddToCurrentMatches()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (!currentMatches.Contains(candyPosition[i, j]) && candyPosition[i, j].GetComponent<Candy>().isMatched)
                    {
                        currentMatches.Add(candyPosition[i, j]);
                    }
                }
            }
        }
    }
    public void GetChain()
    {
        foreach (GameObject candy in currentMatches)
        {
            if (candy.GetComponent<Candy>().isColumnStripe)
            {
                GetColumnCandies(candy.GetComponent<Candy>().atColumn);
            }
            if (candy.GetComponent<Candy>().isRowStripe)
            {
                GetRowCandies(candy.GetComponent<Candy>().atRow);
            }
        }
    }
    public bool IsSpecialCandyInList(List<GameObject> candiesList)
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    GameObject candy = candyPosition[i, j];
                    if (candiesList.Contains(candy) && !candy.GetComponent<Candy>().isMatched)
                    {
                        if (candy.GetComponent<Candy>().isColumnStripe || candy.GetComponent<Candy>().isRowStripe)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public void DestroyCandy()
    {
        int count = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (!candyPosition[i, j].GetComponent<Candy>().isMatched)
                    {
                        count++;
                    }
                }
            }
        }
        if (currentMatches.Count > 0)
        {
            StartCoroutine(DestroyCandyAt());
        }
        else if (count == column * row)
        {
            currentState = GameState.Idling;
        }
    }

    public IEnumerator DestroyCandyAt()
    {
        yield return new WaitForSeconds(awaitTime);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (candyPosition[i, j].GetComponent<Candy>().isMatched)
                    {
                        //if (IsMatchFive())
                        //{
                        //    if (selectedCandy != null)
                        //    {
                        //        GameObject otherCandy = selectedCandy.GetComponent<Candy>().otherCandy;
                        //        if (selectedCandy.GetComponent<Candy>().isMatched)
                        //        {
                        //            selectedCandy.GetComponent<Candy>().isMatched = false;
                        //            selectedCandy.GetComponent<Candy>().ColorBomb();
                        //            currentMatches.Remove(selectedCandy);
                        //        }
                        //        else if (otherCandy.GetComponent<Candy>().isMatched)
                        //        {
                        //            otherCandy.GetComponent<Candy>().isMatched = false;
                        //            otherCandy.GetComponent<Candy>().ColorBomb();
                        //            currentMatches.Remove(otherCandy);
                        //        }
                        //    }
                        //}

                        //if (IsMatchFour())
                        //{
                        //    if (selectedCandy != null)
                        //    {
                        //        GameObject otherCandy = selectedCandy.GetComponent<Candy>().otherCandy;
                        //        if (selectedCandy.GetComponent<Candy>().isMatched)
                        //        {
                        //            selectedCandy.GetComponent<Candy>().isMatched = false;
                        //            if (selectedCandy.transform.position.x == otherCandy.transform.position.x)
                        //            {
                        //                selectedCandy.GetComponent<Candy>().ColumnStripe();
                        //            }
                        //            else
                        //            {
                        //                selectedCandy.GetComponent<Candy>().RowStripe();
                        //            }
                        //            currentMatches.Remove(selectedCandy);
                        //        }
                        //        if (otherCandy.GetComponent<Candy>().isMatched)
                        //        {
                        //            otherCandy.GetComponent<Candy>().isMatched = false;
                        //            if (selectedCandy.transform.position.x == otherCandy.transform.position.x)
                        //            {
                        //                otherCandy.GetComponent<Candy>().ColumnStripe();
                        //            }
                        //            else
                        //            {
                        //                otherCandy.GetComponent<Candy>().RowStripe();
                        //            }
                        //            currentMatches.Remove(otherCandy);
                        //        }
                        //    }
                        //}
                        currentMatches.Remove(candyPosition[i, j]);
                        Destroy(candyPosition[i, j]);
                        candyPosition[i, j] = null;
                    }
                }
            }
        }
        yield return new WaitForSeconds(awaitTime);
        StartCoroutine(CollapseRow());
    }

    private bool IsMatchFour()
    {
        int numHorizontal = 0;
        int numVertical = 0;
        Candy firstCandy = currentMatches[0].GetComponent<Candy>();
        if (firstCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.GetComponent<Candy>().atRow == firstCandy.atRow && candy.name == firstCandy.name)
                {
                    numHorizontal++;
                }
                if (candy.GetComponent<Candy>().atColumn == firstCandy.atColumn && candy.name == firstCandy.name)
                {
                    numVertical++;
                }
            }
        }
        return (numHorizontal == 4 || numVertical == 4);
    }

    //private bool IsMatchFive()
    //{
    //    int numHorizontal = 0;
    //    int numVertical = 0;
    //    Candy firstCandy = currentMatches[0].GetComponent<Candy>();
    //    if (firstCandy != null)
    //    {
    //        foreach (GameObject candy in currentMatches)
    //        {
    //            if (candy.GetComponent<Candy>().atRow == firstCandy.atRow && candy.name == firstCandy.name)
    //            {
    //                numHorizontal++;
    //            }
    //            if (candy.GetComponent<Candy>().atColumn == firstCandy.atColumn && candy.name == firstCandy.name)
    //            {
    //                numVertical++;
    //            }
    //        }
    //    }
    //    return (numHorizontal == 5 || numVertical == 5);
    //}

    public IEnumerator CollapseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    candyPosition[i, j].GetComponent<Candy>().atRow -= nullCount;
                    StartCoroutine(candyPosition[i, j].GetComponent<Candy>().CollapseCandy());
                    candyPosition[i, j].GetComponent<Candy>().GetPosition();
                    candyPosition[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(awaitTime);
        selectedCandy = null;
        currentMatches.Clear();
        RefillBoard();
    }
    public void RefillBoard()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int random = Random.Range(0, candyType.Count);
                    GameObject newCandy = Instantiate(candyType[random], tempPosition, Quaternion.identity);

                    newCandy.GetComponent<Candy>().atRow = j;
                    newCandy.GetComponent<Candy>().atColumn = i;
                    newCandy.transform.parent = this.transform;
                    newCandy.name = newCandy.name.Replace("(Clone)", "");
                    candyPosition[i, j] = newCandy;
                }
            }
        }
        ScanBoard();
    }
    public bool CheckMatchInit(int column, int row, GameObject candy)
    {
        if (column > 1)
        {
            if (candyPosition[column - 1, row].name == candy.name &&
                candyPosition[column - 2, row].name == candy.name)
            {
                return true;
            }
        }
        if (row > 1)
        {
            if (candyPosition[column, row - 1].name == candy.name &&
                candyPosition[column, row - 2].name == candy.name)
            {
                return true;
            }
        }
        return false;
    }
    private void GetColumnCandies(int boardColumn)
    {
        for (int i = 0; i < row; i++)
        {
            if (candyPosition[boardColumn, i] != null && !currentMatches.Contains(candyPosition[boardColumn, i]))
            {
                candyPosition[boardColumn, i].GetComponent<Candy>().isMatched = true;
            }
        }
    }
    private void GetRowCandies(int boardRow)
    {
        for (int i = 0; i < column; i++)
        {
            if (candyPosition[i, boardRow] != null && !currentMatches.Contains(candyPosition[i, boardRow]))
            {
                candyPosition[i, boardRow].GetComponent<Candy>().isMatched = true;
            }
        }
    }
}

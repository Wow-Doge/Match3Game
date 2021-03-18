using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GetChain();
    }
    public bool MatchedCandyNotInList()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (candyPosition[i, j].GetComponent<Candy>().isMatched && !currentMatches.Contains(candyPosition[i, j]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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
            if (candy.GetComponent<Candy>().isSquareBomb)
            {
                GetSquareCandies(candy);
            }
            if (candy.GetComponent<Candy>().isColorBomb)
            {
                //not working as intended
                //GetSameColorCandies(candyType[Random.Range(0, candyType.Count)]);
            }
            
        }
        if (MatchedCandyNotInList())
        {
            AddToCurrentMatches();
        }
        else
        {
            DestroyCandy();
        }
    }

    public void DestroyCandy()
    {
        if (IsMatchedOnBoard())
        {
            StartCoroutine(MatchSpecialCandy());
        }
        else
        {
            currentState = GameState.Idling;
        }
    }

    public bool IsMatchedOnBoard()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (candyPosition[i, j].GetComponent<Candy>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
     
    public IEnumerator MatchSpecialCandy()
    {
        yield return new WaitForSeconds(awaitTime);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null && candyPosition[i, j].GetComponent<Candy>().isMatched)
                {
                    if (selectedCandy != null)
                    {
                        GameObject otherCandy = selectedCandy.GetComponent<Candy>().otherCandy;
                        if (IsPlayerMatchColorBomb(selectedCandy))
                        {
                            selectedCandy.GetComponent<Candy>().isMatched = false;
                            selectedCandy.GetComponent<Candy>().ColorBombCandy();
                        }
                        if (IsPlayerMatchColorBomb(otherCandy))
                        {
                            otherCandy.GetComponent<Candy>().isMatched = false;
                            otherCandy.GetComponent<Candy>().ColorBombCandy();
                        }
                        if (IsPlayerMatchSquareBomb(selectedCandy))
                        {
                            selectedCandy.GetComponent<Candy>().isMatched = false;
                            selectedCandy.GetComponent<Candy>().SquareBombCandy();
                        }
                        if (IsPlayerMatchSquareBomb(otherCandy))
                        {
                            otherCandy.GetComponent<Candy>().isMatched = false;
                            otherCandy.GetComponent<Candy>().SquareBombCandy();
                        }
                        if (IsPlayerMatchStripe(selectedCandy))
                        {
                            selectedCandy.GetComponent<Candy>().isMatched = false;
                            if (selectedCandy.GetComponent<Candy>().atColumn == otherCandy.GetComponent<Candy>().atColumn)
                            {
                                selectedCandy.GetComponent<Candy>().ColumnStripeCandy();
                            }
                            else if (selectedCandy.GetComponent<Candy>().atRow == otherCandy.GetComponent<Candy>().atRow)
                            {
                                selectedCandy.GetComponent<Candy>().RowStripeCandy();
                            }
                        }
                        if (IsPlayerMatchStripe(otherCandy))
                        {
                            otherCandy.GetComponent<Candy>().isMatched = false;
                            if (selectedCandy.GetComponent<Candy>().atRow == otherCandy.GetComponent<Candy>().atRow)
                            {
                                otherCandy.GetComponent<Candy>().RowStripeCandy();
                            }
                            else if (selectedCandy.GetComponent<Candy>().atColumn == otherCandy.GetComponent<Candy>().atColumn)
                            {
                                otherCandy.GetComponent<Candy>().ColumnStripeCandy();
                            }
                        }
                    }
                    else
                    {
                        if (IsBoardMatchColumnColorBomb(candyPosition[i, j]))
                        {
                            candyPosition[i, j].GetComponent<Candy>().isMatched = false;
                            candyPosition[i, j].GetComponent<Candy>().ColorBombCandy();
                        }
                        if (IsBoardMatchRowColorBomb(candyPosition[i, j]))
                        {
                            candyPosition[i, j].GetComponent<Candy>().isMatched = false;
                            candyPosition[i, j].GetComponent<Candy>().ColorBombCandy();
                        }
                        if (IsPlayerMatchSquareBomb(candyPosition[i, j]))
                        {
                            candyPosition[i, j].GetComponent<Candy>().isMatched = false;
                            candyPosition[i, j].GetComponent<Candy>().SquareBombCandy();
                        }
                        if (IsBoardMatchColumnStripe(candyPosition[i, j]))
                        {
                            candyPosition[i, j].GetComponent<Candy>().isMatched = false;
                            candyPosition[i, j].GetComponent<Candy>().ColumnStripeCandy();
                        }
                        if (IsBoardMatchRowStripe(candyPosition[i, j]))
                        {
                            candyPosition[i, j].GetComponent<Candy>().isMatched = false;
                            candyPosition[i, j].GetComponent<Candy>().RowStripeCandy();
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(awaitTime);
        RemoveMatchedCandies();
    }

    private void RemoveMatchedCandies()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (candyPosition[i, j].GetComponent<Candy>().isMatched)
                    {
                        currentMatches.Remove(candyPosition[i, j]);
                        Destroy(candyPosition[i, j]);
                        candyPosition[i, j] = null;
                    }
                }
            }
        }
        StartCoroutine(CollapseRow());
    }

    private bool IsPlayerMatchColorBomb(GameObject targetCandy)
    {
        List<GameObject> horizontalList = new List<GameObject>();
        List<GameObject> verticalList = new List<GameObject>();
        if (targetCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.GetComponent<Candy>().atColumn == targetCandy.GetComponent<Candy>().atColumn && candy.name == targetCandy.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().atRow - targetCandy.GetComponent<Candy>().atRow) < 3)
                    {
                        verticalList.Add(candy);
                    }
                }
                if (candy.GetComponent<Candy>().atRow == targetCandy.GetComponent<Candy>().atRow && candy.name == targetCandy.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().atColumn - targetCandy.GetComponent<Candy>().atColumn) < 3)
                    {
                        horizontalList.Add(candy);
                    }
                }
            }
        }

        if (verticalList.Count == 5)
        {
            foreach (GameObject candy in verticalList)
            {
                currentMatches.Remove(candy);
            }
        }
        if (horizontalList.Count == 5)
        {
            foreach (GameObject candy in horizontalList)
            {
                currentMatches.Remove(candy);
            }
        }
        return (verticalList.Count == 5 || horizontalList.Count == 5);
    }
    private bool IsPlayerMatchStripe(GameObject targetCandy)
    {
        List<GameObject> horizontalList = new List<GameObject>();
        List<GameObject> verticalList = new List<GameObject>();
        if (targetCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.GetComponent<Candy>().atColumn == targetCandy.GetComponent<Candy>().atColumn && candy.name == targetCandy.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().atRow - targetCandy.GetComponent<Candy>().atRow) < 3)
                    {
                        verticalList.Add(candy);
                    }
                }
                if (candy.GetComponent<Candy>().atRow == targetCandy.GetComponent<Candy>().atRow && candy.name == targetCandy.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().atColumn - targetCandy.GetComponent<Candy>().atColumn) < 3)
                    {
                        horizontalList.Add(candy);
                    }
                }
            }
        }
        if (verticalList.Count == 4)
        {
            foreach (GameObject candy in verticalList)
            {
                currentMatches.Remove(candy);
            }
        }
        if (horizontalList.Count == 4)
        {
            foreach (GameObject candy in horizontalList)
            {
                currentMatches.Remove(candy);
            }
        }
        return (verticalList.Count == 4 || horizontalList.Count == 4);
    }
    private bool IsPlayerMatchSquareBomb(GameObject targetCandy)
    {
        List<GameObject> horizontalList = new List<GameObject>();
        List<GameObject> verticalList = new List<GameObject>();
        if (targetCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.name == targetCandy.name)
                {
                    if (candy.GetComponent<Candy>().atRow == targetCandy.GetComponent<Candy>().atRow)
                    {
                        if (Mathf.Abs(candy.GetComponent<Candy>().atColumn - targetCandy.GetComponent<Candy>().atColumn) < 3)
                        {
                            horizontalList.Add(candy);
                        }
                    }
                    if (candy.GetComponent<Candy>().atColumn == targetCandy.GetComponent<Candy>().atColumn)
                    {
                        if (Mathf.Abs(candy.GetComponent<Candy>().atRow - targetCandy.GetComponent<Candy>().atRow) < 3)
                        {
                            verticalList.Add(candy);
                        }
                    }
                }
            }
        }
        if ((verticalList.Count == 3 || verticalList.Count == 4) && (horizontalList.Count == 3 || horizontalList.Count == 4))
        {
            foreach (GameObject candy in verticalList)
            {
                if (candy != null)
                {
                    currentMatches.Remove(candy);
                }
            }
            foreach (GameObject candy in horizontalList)
            {
                if (candy != null)
                {
                    currentMatches.Remove(candy);
                }
            }
        }
        return (verticalList.Count == 4 || verticalList.Count == 3) && (horizontalList.Count == 4 || horizontalList.Count == 3);
    }
    private bool IsBoardMatchColumnStripe(GameObject targetCandy)
    {
        List<GameObject> verticalList = new List<GameObject>();
        if (targetCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.GetComponent<Candy>().atColumn == targetCandy.GetComponent<Candy>().atColumn && candy.name == targetCandy.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().atRow - targetCandy.GetComponent<Candy>().atRow) < 3)
                    {
                        verticalList.Add(candy);
                    }
                }
            }
        }
        if (verticalList.Count == 4)
        {
            foreach (GameObject candy in verticalList)
            {
                currentMatches.Remove(candy);
            }
        }
        return verticalList.Count == 4;
    }
    private bool IsBoardMatchRowStripe(GameObject targetCandy)
    {
        List<GameObject> horizontalList = new List<GameObject>();
        if (targetCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.GetComponent<Candy>().atRow == targetCandy.GetComponent<Candy>().atRow && candy.name == targetCandy.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().atColumn - targetCandy.GetComponent<Candy>().atColumn) < 3)
                    {
                        horizontalList.Add(candy);
                    }
                }
            }
        }
        if (horizontalList.Count == 4)
        {
            foreach (GameObject candy in horizontalList)
            {
                currentMatches.Remove(candy);
            }
        }
        return horizontalList.Count == 4;
    }
    private bool IsBoardMatchColumnColorBomb(GameObject targetCandy)
    {
        List<GameObject> verticalList = new List<GameObject>();
        if (targetCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.GetComponent<Candy>().atColumn == targetCandy.GetComponent<Candy>().atColumn && candy.name == targetCandy.name)
                {
                    if (candy.GetComponent<Candy>().atRow - targetCandy.GetComponent<Candy>().atRow < 5)
                    {
                        verticalList.Add(candy);
                    }
                }
            }
        }
        if (verticalList.Count == 5)
        {
            foreach (GameObject candy in verticalList)
            {
                currentMatches.Remove(candy);
            }
        }
        return verticalList.Count == 5;
    }
    private bool IsBoardMatchRowColorBomb(GameObject targetCandy)
    {
        List<GameObject> horizontalList = new List<GameObject>();
        if (targetCandy != null)
        {
            foreach (GameObject candy in currentMatches)
            {
                if (candy.GetComponent<Candy>().atRow == targetCandy.GetComponent<Candy>().atRow && candy.name == targetCandy.name)
                {
                    if (candy.GetComponent<Candy>().atColumn - targetCandy.GetComponent<Candy>().atColumn < 5)
                    {
                        horizontalList.Add(candy);
                    }
                }
            }
        }
        if (horizontalList.Count == 5)
        {
            foreach (GameObject candy in horizontalList)
            {
                currentMatches.Remove(candy);
            }
        }
        return horizontalList.Count == 5;
    }

    public IEnumerator CollapseRow()
    {
        yield return new WaitForSeconds(awaitTime);
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
    public void GetSameColorCandies(GameObject candy)
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (candyPosition[i, j].name == candy.name)
                    {
                        candyPosition[i, j].GetComponent<Candy>().isMatched = true;
                    }
                }
            }
        }
    }
    public void GetSquareCandies(GameObject candy)
    {
        if (candy != null)
        {
            int x = candy.GetComponent<Candy>().atColumn;
            int y = candy.GetComponent<Candy>().atRow;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < column && j >= 0 && j < row)
                    {
                        candyPosition[i, j].GetComponent<Candy>().isMatched = true;
                    }
                }
            }
        }
    }
}

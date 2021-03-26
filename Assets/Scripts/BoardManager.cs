using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Idling,
    Moving
}

public enum BackgroundTileType
{
    Breakable,
    Blank,
    Frozen,
    Lava,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public BackgroundTileType backgroundTileType;
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public GameState currentState = GameState.Idling;
    public int boardHeight;
    public int boardWidth;
    public int offset;
    public float awaitTime = 0.25f;

    public GameObject[,] candyPosition;
    public List<GameObject> candyType = new List<GameObject>();
    public List<GameObject> currentMatches = new List<GameObject>();
    public GameObject selectedCandy;

    public TileType[] boardLayout;
    private bool[,] blankSpaces;

    private BackgroundTile[,] breakableTiles;
    public GameObject breakableTilePrefab;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        candyPosition = new GameObject[boardWidth, boardHeight];
        blankSpaces = new bool[boardWidth, boardHeight];
        breakableTiles = new BackgroundTile[boardWidth, boardHeight];
        CreateBoard();
    }

    public void GenerateBackgroundTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].backgroundTileType == BackgroundTileType.Breakable)
            {
                Vector2 tempPos = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPos, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
            if (boardLayout[i].backgroundTileType == BackgroundTileType.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void CreateBoard()
    {
        GenerateBackgroundTiles();
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (!blankSpaces[i, j])
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
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }

    public void ScanBoard()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                GameObject currentCandy = candyPosition[i, j];
                if (currentCandy != null)
                {
                    if (i > 0 && i < boardWidth - 1)
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

                    if (j > 0 && j < boardHeight - 1)
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
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
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
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
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
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
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
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
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
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (candyPosition[i, j].GetComponent<Candy>().isMatched)
                    {
                        if (breakableTiles[i, j] != null)
                        {
                            breakableTiles[i, j].TakeDamage(1);
                            if (breakableTiles[i, j].hitPoints <= 0)
                            {
                                Destroy(breakableTiles[i, j].gameObject);
                                breakableTiles[i, j] = null;
                            }
                        }
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

    private IEnumerator CollapseRow()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (!blankSpaces[i, j] && candyPosition[i, j] == null)
                {
                    for (int k = j + 1; k < boardHeight; k++)
                    {
                        if (candyPosition[i, k] != null)
                        {
                            candyPosition[i, k].GetComponent<Candy>().atRow = j;
                            StartCoroutine(candyPosition[i, k].GetComponent<Candy>().CollapseCandy());
                            candyPosition[i, k].GetComponent<Candy>().GetPosition();
                            candyPosition[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(awaitTime);
        selectedCandy = null;
        currentMatches.Clear();
        RefillBoard();
    }

    //public IEnumerator CollapseRow()
    //{
    //    yield return new WaitForSeconds(awaitTime);
    //    int nullCount = 0;
    //    for (int i = 0; i < boardWidth; i++)
    //    {
    //        for (int j = 0; j < boardHeight; j++)
    //        {
    //            if (candyPosition[i, j] == null)
    //            {
    //                nullCount++;
    //            }
    //            else if (nullCount > 0)
    //            {
    //                candyPosition[i, j].GetComponent<Candy>().atRow -= nullCount;
    //                StartCoroutine(candyPosition[i, j].GetComponent<Candy>().CollapseCandy());
    //                candyPosition[i, j].GetComponent<Candy>().GetPosition();
    //                candyPosition[i, j] = null;
    //            }
    //        }
    //        nullCount = 0;
    //    }
    //    yield return new WaitForSeconds(awaitTime);
    //    selectedCandy = null;
    //    currentMatches.Clear();
    //    RefillBoard();
    //}
    public void RefillBoard()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (candyPosition[i, j] == null && !blankSpaces[i, j])
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
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
        ScanBoard();
    }
    public bool CheckMatchInit(int boardHeight, int boardWidth, GameObject candy)
    {
        if (boardHeight > 1)
        {
            if (candyPosition[boardHeight - 1, boardWidth] != null && candyPosition[boardHeight - 2, boardWidth] != null)
            {
                if (candyPosition[boardHeight - 1, boardWidth].name == candy.name &&
                    candyPosition[boardHeight - 2, boardWidth].name == candy.name)
                {
                    return true;
                }
            }
        }
        if (boardWidth > 1)
        {
            if (candyPosition[boardHeight, boardWidth - 1] != null && candyPosition[boardHeight, boardWidth - 2] != null)
            {
                if (candyPosition[boardHeight, boardWidth - 1].name == candy.name &&
                    candyPosition[boardHeight, boardWidth - 2].name == candy.name)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void GetColumnCandies(int boardColumn)
    {
        for (int i = 0; i < boardHeight; i++)
        {
            if (candyPosition[boardColumn, i] != null && !currentMatches.Contains(candyPosition[boardColumn, i]))
            {
                candyPosition[boardColumn, i].GetComponent<Candy>().isMatched = true;
            }
        }
    }
    private void GetRowCandies(int boardRow)
    {
        for (int i = 0; i < boardWidth; i++)
        {
            if (candyPosition[i, boardRow] != null && !currentMatches.Contains(candyPosition[i, boardRow]))
            {
                candyPosition[i, boardRow].GetComponent<Candy>().isMatched = true;
            }
        }
    }
    public void GetSameColorCandies(GameObject candy)
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
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
                    if (i >= 0 && i < boardWidth && j >= 0 && j < boardHeight)
                    {
                        if (candyPosition[i, j] != null)
                        {
                            candyPosition[i, j].GetComponent<Candy>().isMatched = true;
                        }
                    }
                }
            }
        }
    }

    private void SwitchPieces(int boardHeight, int boardWidth, Vector2 direction)
    {
        GameObject holder = candyPosition[boardHeight + (int)direction.x, boardWidth + (int)direction.y] as GameObject;
        candyPosition[boardHeight + (int)direction.x, boardWidth + (int)direction.y] = candyPosition[boardHeight, boardWidth];
        candyPosition[boardHeight, boardWidth] = holder;
    }
    private bool CheckForMatches()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (i < boardWidth - 2)
                    {
                        if (candyPosition[i + 1, j] != null && candyPosition[i + 2, j] != null)
                        {
                            if (candyPosition[i + 1, j].name == candyPosition[i, j].name && candyPosition[i + 2, j].name == candyPosition[i, j].name)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < boardHeight - 2)
                    {
                        if (candyPosition[i, j + 1] != null && candyPosition[i, j + 2] != null)
                        {
                            if (candyPosition[i, j + 1].name == candyPosition[i, j].name && candyPosition[i, j + 2].name == candyPosition[i, j].name)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    private bool SwitchAndCheck(int boardHeight, int boardWidth, Vector2 direction)
    {
        SwitchPieces(boardHeight, boardWidth, direction);
        if (CheckForMatches())
        {
            SwitchPieces(boardHeight, boardWidth, direction);
            return true;
        }
        SwitchPieces(boardHeight, boardWidth, direction);
        return false;
    }
    private bool IsDeadLocked()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (candyPosition[i, j].name == "rainbow")
                    {
                        return false;
                    }
                    if (i < boardWidth - 1)
                    {
                        if (candyPosition[i + 1, j] != null)
                        {
                            if (SwitchAndCheck(i, j, Vector2.right))
                            {
                                return false;
                            }
                        }
                    }
                    if (j < boardHeight - 1)
                    {
                        if (candyPosition[i, j + 1] != null)
                        {
                            if (SwitchAndCheck(i, j, Vector2.up))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return true;
    }

    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    newBoard.Add(candyPosition[i, j]);
                }
            }
        }
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (!blankSpaces[i, j])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    int random = Random.Range(0, candyType.Count);
                    int maxIterations = 0;
                    while (CheckMatchInit(i, j, candyType[random]) && maxIterations < 100)
                    {
                        random = Random.Range(0, candyType.Count);
                        maxIterations++;
                    }
                    Candy candy = newBoard[pieceToUse].GetComponent<Candy>();
                    maxIterations = 0;
                    candy.atColumn = i;
                    candy.atRow = j;
                    candyPosition[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                    candy.transform.position = new Vector2(i, j);
                }
            }
        }
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }
}


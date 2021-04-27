using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

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
    public int height;
    public int width;
    public int offset;
    public float delayTime = 0.25f;

    public GameObject[,] candyPos;
    public List<GameObject> candyType = new List<GameObject>();
    public List<GameObject> matchedList = new List<GameObject>();
    public GameObject selectCandy;

    public TileType[] boardLayout;
    private bool[,] blankSpaces;

    private BackgroundTile[,] breakableTiles;
    public GameObject breakableTilePrefab;

    public int basePieceValue = 20;
    private int streakValue = 0;
    private int totalScore;

    public Dictionary<string, List<GameObject>> dict = new Dictionary<string, List<GameObject>>();
    public GameObject battleSystem;

    public event EventHandler OnTurnEnd;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        candyPos = new GameObject[width, height];
        blankSpaces = new bool[width, height];
        breakableTiles = new BackgroundTile[width, height];
        CreateBoard();
        CreateDict();
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
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int random = Random.Range(0, candyType.Count);
                    int maxIterations = 0;
                    while (CheckMatchInit(i, j, candyType[random]) && maxIterations < 100)
                    {
                        random = Random.Range(0, candyType.Count);
                        maxIterations++;
                    }
                    GameObject newCandy = Instantiate(candyType[random], tempPos, Quaternion.identity);
                    newCandy.GetComponent<Candy>().row = j;
                    newCandy.GetComponent<Candy>().column = i;
                    newCandy.transform.parent = this.transform;
                    newCandy.name = newCandy.name.Replace("(Clone)", "");
                    candyPos[i, j] = newCandy;
                }
            }
        }
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }
    public void FindMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject select = candyPos[i, j];
                if (select != null)
                {
                    if (i > 0 && i < width - 1)
                    {
                        GameObject left = candyPos[i - 1, j];
                        GameObject right = candyPos[i + 1, j];
                        if (left != null && right != null)
                        {
                            if (left.name == select.name && right.name == select.name)
                            {
                                select.GetComponent<Candy>().isMatched = true;
                                left.GetComponent<Candy>().isMatched = true;
                                right.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < height - 1)
                    {
                        GameObject down = candyPos[i, j - 1];
                        GameObject up = candyPos[i, j + 1];
                        if (down != null && up != null)
                        {
                            if (down.name == select.name && up.name == down.name)
                            {
                                select.GetComponent<Candy>().isMatched = true;
                                up.GetComponent<Candy>().isMatched = true;
                                down.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
        if (IsMatchedOnBoard())
        {
            AddToMatchedList();
        }
        else
        {
            if (streakValue == 0)
            {
                return;
            }
            OnTurnEnd?.Invoke(this, EventArgs.Empty);
            streakValue = 0;
            ClearDict();
        }
    }
    public void AddToMatchedList()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    if (!matchedList.Contains(candyPos[i, j]) && candyPos[i, j].GetComponent<Candy>().isMatched)
                    {
                        matchedList.Add(candyPos[i, j]);
                    }
                }
            }
        }
        GetChain();
    }
    public bool MatchedCandyNotInList()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    if (candyPos[i, j].GetComponent<Candy>().isMatched && !matchedList.Contains(candyPos[i, j]))
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
        foreach (GameObject candy in matchedList)
        {
            if (candy.GetComponent<Candy>().special == Candy.SpecialCandy.ColumnStripe)
            {
                GetColumn(candy.GetComponent<Candy>().column);
            }
            if (candy.GetComponent<Candy>().special == Candy.SpecialCandy.RowStripe)
            {
                GetRow(candy.GetComponent<Candy>().row);
            }
            if (candy.GetComponent<Candy>().special == Candy.SpecialCandy.SquareBomb)
            {
                GetSquare(candy);
            }
            if (candy.GetComponent<Candy>().special == Candy.SpecialCandy.ColorBomb)
            {
                //not working as intended
                //GetSameColorCandies(candyType[Random.Range(0, candyType.Count)]);
            }
            
        }
        if (MatchedCandyNotInList())
        {
            AddToMatchedList();
        }
        else
        {
            StartCoroutine(MatchSpecial());
        }
    }
    public bool IsMatchedOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    if (candyPos[i, j].GetComponent<Candy>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public IEnumerator MatchSpecial()
    {
        CountColor();
        yield return new WaitForSeconds(delayTime);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null && candyPos[i, j].GetComponent<Candy>().isMatched)
                {
                    if (selectCandy != null)
                    {
                        GameObject other = selectCandy.GetComponent<Candy>().otherCandy;
                        if (PlayerMatchColor(selectCandy))
                        {
                            selectCandy.GetComponent<Candy>().ColorBomb();
                        }
                        if (PlayerMatchColor(other))
                        {
                            other.GetComponent<Candy>().ColorBomb();
                        }
                        if (PlayerMatchSquare(selectCandy))
                        {
                            selectCandy.GetComponent<Candy>().SquareBomb();
                        }
                        if (PlayerMatchSquare(other))
                        {
                            other.GetComponent<Candy>().SquareBomb();
                        }
                        if (PlayerMatchStripe(selectCandy))
                        {
                            if (selectCandy.GetComponent<Candy>().column == other.GetComponent<Candy>().column)
                            {
                                selectCandy.GetComponent<Candy>().ColumnStripe();
                            }
                            else if (selectCandy.GetComponent<Candy>().row == other.GetComponent<Candy>().row)
                            {
                                selectCandy.GetComponent<Candy>().RowStripe();
                            }
                        }
                        if (PlayerMatchStripe(other))
                        {
                            if (selectCandy.GetComponent<Candy>().row == other.GetComponent<Candy>().row)
                            {
                                other.GetComponent<Candy>().RowStripe();
                            }
                            else if (selectCandy.GetComponent<Candy>().column == other.GetComponent<Candy>().column)
                            {
                                other.GetComponent<Candy>().ColumnStripe();
                            }
                        }
                    }
                    else
                    {
                        if (BoardMatchColumnColor(candyPos[i, j]))
                        {
                            candyPos[i, j].GetComponent<Candy>().ColorBomb();
                        }
                        if (BoardMatchRowColor(candyPos[i, j]))
                        {
                            candyPos[i, j].GetComponent<Candy>().ColorBomb();
                        }
                        if (PlayerMatchSquare(candyPos[i, j]))
                        {
                            candyPos[i, j].GetComponent<Candy>().SquareBomb();
                        }
                        if (BoardMatchColumnStripe(candyPos[i, j]))
                        {
                            candyPos[i, j].GetComponent<Candy>().ColumnStripe();
                        }
                        if (BoardMatchRowStripe(candyPos[i, j]))
                        {
                            candyPos[i, j].GetComponent<Candy>().RowStripe();
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(delayTime);
        RemoveMatched();
    }

    private void RemoveMatched()
    {
        totalScore = 0;
        streakValue++;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    if (candyPos[i, j].GetComponent<Candy>().isMatched)
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
                        totalScore += basePieceValue * streakValue;
                        matchedList.Remove(candyPos[i, j]);
                        Destroy(candyPos[i, j]);
                        candyPos[i, j] = null;
                    }
                }
            }
        }
        //ScoreManager.Instance.IncreaseScore(totalScore);
        StartCoroutine(CollapseRow());
    }

    private bool PlayerMatchColor(GameObject target)
    {
        List<GameObject> horizontal = new List<GameObject>();
        List<GameObject> vertical = new List<GameObject>();
        if (target != null)
        {
            foreach (GameObject candy in matchedList)
            {
                if (candy.GetComponent<Candy>().column == target.GetComponent<Candy>().column && candy.name == target.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().row - target.GetComponent<Candy>().row) < 3)
                    {
                        vertical.Add(candy);
                    }
                }
                if (candy.GetComponent<Candy>().row == target.GetComponent<Candy>().row && candy.name == target.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().column - target.GetComponent<Candy>().column) < 3)
                    {
                        horizontal.Add(candy);
                    }
                }
            }
        }

        if (vertical.Count == 5)
        {
            foreach (GameObject candy in vertical)
            {
                matchedList.Remove(candy);
            }
        }
        if (horizontal.Count == 5)
        {
            foreach (GameObject candy in horizontal)
            {
                matchedList.Remove(candy);
            }
        }
        return (vertical.Count == 5 || horizontal.Count == 5);
    }
    private bool PlayerMatchStripe(GameObject target)
    {
        List<GameObject> horizontal = new List<GameObject>();
        List<GameObject> vertical = new List<GameObject>();
        if (target != null)
        {
            foreach (GameObject candy in matchedList)
            {
                if (candy.GetComponent<Candy>().column == target.GetComponent<Candy>().column && candy.name == target.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().row - target.GetComponent<Candy>().row) < 3)
                    {
                        vertical.Add(candy);
                    }
                }
                if (candy.GetComponent<Candy>().row == target.GetComponent<Candy>().row && candy.name == target.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().column - target.GetComponent<Candy>().column) < 3)
                    {
                        horizontal.Add(candy);
                    }
                }
            }
        }
        if (vertical.Count == 4)
        {
            foreach (GameObject candy in vertical)
            {
                matchedList.Remove(candy);
            }
        }
        if (horizontal.Count == 4)
        {
            foreach (GameObject candy in horizontal)
            {
                matchedList.Remove(candy);
            }
        }
        return (vertical.Count == 4 || horizontal.Count == 4);
    }
    private bool PlayerMatchSquare(GameObject target)
    {
        List<GameObject> horizontal = new List<GameObject>();
        List<GameObject> vertical = new List<GameObject>();
        if (target != null)
        {
            foreach (GameObject candy in matchedList)
            {
                if (candy.name == target.name)
                {
                    if (candy.GetComponent<Candy>().row == target.GetComponent<Candy>().row)
                    {
                        if (Mathf.Abs(candy.GetComponent<Candy>().column - target.GetComponent<Candy>().column) < 3)
                        {
                            horizontal.Add(candy);
                        }
                    }
                    if (candy.GetComponent<Candy>().column == target.GetComponent<Candy>().column)
                    {
                        if (Mathf.Abs(candy.GetComponent<Candy>().row - target.GetComponent<Candy>().row) < 3)
                        {
                            vertical.Add(candy);
                        }
                    }
                }
            }
        }
        if ((vertical.Count == 3 || vertical.Count == 4) && (horizontal.Count == 3 || horizontal.Count == 4))
        {
            foreach (GameObject candy in vertical)
            {
                if (candy != null)
                {
                    matchedList.Remove(candy);
                }
            }
            foreach (GameObject candy in horizontal)
            {
                if (candy != null)
                {
                    matchedList.Remove(candy);
                }
            }
        }
        return (vertical.Count == 4 || vertical.Count == 3) && (horizontal.Count == 4 || horizontal.Count == 3);
    }
    private bool BoardMatchColumnStripe(GameObject target)
    {
        List<GameObject> vertical = new List<GameObject>();
        if (target != null)
        {
            foreach (GameObject candy in matchedList)
            {
                if (candy.GetComponent<Candy>().column == target.GetComponent<Candy>().column && candy.name == target.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().row - target.GetComponent<Candy>().row) < 3)
                    {
                        vertical.Add(candy);
                    }
                }
            }
        }
        if (vertical.Count == 4)
        {
            foreach (GameObject candy in vertical)
            {
                matchedList.Remove(candy);
            }
        }
        return vertical.Count == 4;
    }
    private bool BoardMatchRowStripe(GameObject target)
    {
        List<GameObject> horizontal = new List<GameObject>();
        if (target != null)
        {
            foreach (GameObject candy in matchedList)
            {
                if (candy.GetComponent<Candy>().row == target.GetComponent<Candy>().row && candy.name == target.name)
                {
                    if (Mathf.Abs(candy.GetComponent<Candy>().column - target.GetComponent<Candy>().column) < 3)
                    {
                        horizontal.Add(candy);
                    }
                }
            }
        }
        if (horizontal.Count == 4)
        {
            foreach (GameObject candy in horizontal)
            {
                matchedList.Remove(candy);
            }
        }
        return horizontal.Count == 4;
    }
    private bool BoardMatchColumnColor(GameObject target)
    {
        List<GameObject> vertical = new List<GameObject>();
        if (target != null)
        {
            foreach (GameObject candy in matchedList)
            {
                if (candy.GetComponent<Candy>().column == target.GetComponent<Candy>().column && candy.name == target.name)
                {
                    if (candy.GetComponent<Candy>().row - target.GetComponent<Candy>().row < 5)
                    {
                        vertical.Add(candy);
                    }
                }
            }
        }
        if (vertical.Count == 5)
        {
            foreach (GameObject candy in vertical)
            {
                matchedList.Remove(candy);
            }
        }
        return vertical.Count == 5;
    }
    private bool BoardMatchRowColor(GameObject target)
    {
        List<GameObject> horizontal = new List<GameObject>();
        if (target != null)
        {
            foreach (GameObject candy in matchedList)
            {
                if (candy.GetComponent<Candy>().row == target.GetComponent<Candy>().row && candy.name == target.name)
                {
                    if (candy.GetComponent<Candy>().column - target.GetComponent<Candy>().column < 5)
                    {
                        horizontal.Add(candy);
                    }
                }
            }
        }
        if (horizontal.Count == 5)
        {
            foreach (GameObject candy in horizontal)
            {
                matchedList.Remove(candy);
            }
        }
        return horizontal.Count == 5;
    }

    private IEnumerator CollapseRow()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && candyPos[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (candyPos[i, k] != null)
                        {
                            candyPos[i, k].GetComponent<Candy>().row = j;
                            StartCoroutine(candyPos[i, k].GetComponent<Candy>().CollapseCandy());
                            candyPos[i, k].GetComponent<Candy>().GetPos();
                            candyPos[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(delayTime);
        selectCandy = null;
        matchedList.Clear();
        RefillBoard();
    }
    public void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int random = Random.Range(0, candyType.Count);
                    GameObject newCandy = Instantiate(candyType[random], tempPos, Quaternion.identity);

                    newCandy.GetComponent<Candy>().row = j;
                    newCandy.GetComponent<Candy>().column = i;
                    newCandy.transform.parent = this.transform;
                    newCandy.name = newCandy.name.Replace("(Clone)", "");
                    candyPos[i, j] = newCandy;
                }
            }
        }
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
        FindMatches();
    }

    public bool CheckMatchInit(int Y, int X, GameObject candy)
    {
        if (Y > 1)
        {
            if (candyPos[Y - 1, X] != null && candyPos[Y - 2, X] != null)
            {
                if (candyPos[Y - 1, X].name == candy.name &&
                    candyPos[Y - 2, X].name == candy.name)
                {
                    return true;
                }
            }
        }
        if (X > 1)
        {
            if (candyPos[Y, X - 1] != null && candyPos[Y, X - 2] != null)
            {
                if (candyPos[Y, X - 1].name == candy.name &&
                    candyPos[Y, X - 2].name == candy.name)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void GetColumn(int X)
    {
        for (int i = 0; i < height; i++)
        {
            if (candyPos[X, i] != null && !matchedList.Contains(candyPos[X, i]))
            {
                candyPos[X, i].GetComponent<Candy>().isMatched = true;
            }
        }
    }
    private void GetRow(int Y)
    {
        for (int i = 0; i < width; i++)
        {
            if (candyPos[i, Y] != null && !matchedList.Contains(candyPos[i, Y]))
            {
                candyPos[i, Y].GetComponent<Candy>().isMatched = true;
            }
        }
    }
    public void GetColor(GameObject candy)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    if (candyPos[i, j].name == candy.name)
                    {
                        candyPos[i, j].GetComponent<Candy>().isMatched = true;
                    }
                }
            }
        }
    }
    public void GetSquare(GameObject candy)
    {
        if (candy != null)
        {
            int x = candy.GetComponent<Candy>().column;
            int y = candy.GetComponent<Candy>().row;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < width && j >= 0 && j < height)
                    {
                        if (candyPos[i, j] != null)
                        {
                            candyPos[i, j].GetComponent<Candy>().isMatched = true;
                        }
                    }
                }
            }
        }
    }

    private void SwitchPieces(int column, int row, Vector2 dir)
    {
        GameObject holder = candyPos[column + (int)dir.x, row + (int)dir.y] as GameObject;
        candyPos[column + (int)dir.x, row + (int)dir.y] = candyPos[column, row];
        candyPos[column, row] = holder;
    }
    private bool CheckPossibleMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    if (i < width - 2)
                    {
                        if (candyPos[i + 1, j] != null && candyPos[i + 2, j] != null)
                        {
                            if (candyPos[i + 1, j].name == candyPos[i, j].name && candyPos[i + 2, j].name == candyPos[i, j].name)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (candyPos[i, j + 1] != null && candyPos[i, j + 2] != null)
                        {
                            if (candyPos[i, j + 1].name == candyPos[i, j].name && candyPos[i, j + 2].name == candyPos[i, j].name)
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
    private bool SwitchAndCheck(int col, int row, Vector2 dir)
    {
        SwitchPieces(col, row, dir);
        if (CheckPossibleMatches())
        {
            SwitchPieces(col, row, dir);
            return true;
        }
        SwitchPieces(col, row, dir);
        return false;
    }
    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    if (candyPos[i, j].name == "rainbow")
                    {
                        return false;
                    }
                    if (i < width - 1)
                    {
                        if (candyPos[i + 1, j] != null)
                        {
                            if (SwitchAndCheck(i, j, Vector2.right))
                            {
                                return false;
                            }
                        }
                    }
                    if (j < height - 1)
                    {
                        if (candyPos[i, j + 1] != null)
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
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (candyPos[i, j] != null)
                {
                    newBoard.Add(candyPos[i, j]);
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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
                    candy.column = i;
                    candy.row = j;
                    candyPos[i, j] = newBoard[pieceToUse];
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

    private Dictionary<string, List<GameObject>> CreateDict()
    {
        foreach (GameObject color in candyType)
        {
            List<GameObject> colorList = new List<GameObject>();
            dict.Add(color.name, colorList);
        }
        return dict;
    }
    private void AddToDict()
    {
        foreach (GameObject candy in matchedList)
        {
            foreach (KeyValuePair<string, List<GameObject>> pair in dict)
            {
                if (candy.name == pair.Key)
                {
                    pair.Value.Add(candy);
                }
            }
        }
    }
    private void ClearDict()
    {
        foreach (var candyColor in dict)
        {
            candyColor.Value.Clear();
        }
    }
    private void CountColor()
    {
        AddToDict();
        //ScoreManager.Instance.ToText();
    }
}


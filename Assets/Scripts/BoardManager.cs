using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public int column;
    public int row;
    public float awaitTime = 0.25f;

    public GameObject[,] candyPosition;

    public List<GameObject> candyType = new List<GameObject>();

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
                Vector2 tempPosition = new Vector2(i, j);
                int random = Random.Range(0, candyType.Count);

                int maxIterations = 0;
                while (CheckMatchInit(i, j, candyType[random]))
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

    public void ScanForMatches()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (i >= 0 && i < row - 2)
                    {
                        if (candyPosition[i + 1, j] != null && candyPosition[i + 2, j] != null)
                        {
                            if (candyPosition[i + 1, j].name == candyPosition[i, j].name && candyPosition[i + 2, j].name == candyPosition[i, j].name)
                            {
                                candyPosition[i, j].GetComponent<Candy>().isMatched = true;
                                candyPosition[i + 1, j].GetComponent<Candy>().isMatched = true;
                                candyPosition[i + 2, j].GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }

                    if (j >= 0 && j < column - 2)
                    {
                        if (candyPosition[i, j + 1] != null && candyPosition[i, j + 2] != null)
                        {
                            if (candyPosition[i, j + 1].name == candyPosition[i, j].name && candyPosition[i, j + 2].name == candyPosition[i, j].name)
                            {
                                candyPosition[i, j].GetComponent<Candy>().isMatched = true;
                                candyPosition[i, j + 1].GetComponent<Candy>().isMatched = true;
                                candyPosition[i, j + 2].GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
        CheckPossibleMatch();
    }
    public void CheckPossibleMatch()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (candyPosition[i, j] != null)
                {
                    if (!candyPosition[i, j].GetComponent<Candy>().isMatched)
                    {
                        //change state
                    }
                    else
                    {
                        StartCoroutine(DestroyMatches());
                    }
                }
            }
        }
    }

    public IEnumerator DestroyMatches()
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
                        Destroy(candyPosition[i, j]);
                        candyPosition[i, j] = null;
                    }
                }
            }
        }
        yield return new WaitForSeconds(awaitTime);
        StartCoroutine(CollapseRow());
    }

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
        ScanForMatches();
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

    public void RefillBoard()
    {

    }
}

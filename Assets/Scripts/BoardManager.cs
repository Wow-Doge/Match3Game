﻿using System.Collections;
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
                                if (!currentMatches.Contains(currentCandy))
                                {
                                    currentMatches.Add(currentCandy);
                                }
                                currentCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(leftCandy))
                                {
                                    currentMatches.Add(leftCandy);
                                }
                                leftCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(rightCandy))
                                {
                                    currentMatches.Add(rightCandy);
                                }
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
                                if (!currentMatches.Contains(upCandy))
                                {
                                    currentMatches.Add(upCandy);
                                }
                                upCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(downCandy))
                                {
                                    currentMatches.Add(downCandy);
                                }
                                downCandy.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
        GetStripeChain();
        DestroyCandy();
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
                currentMatches.Add(candyPosition[boardColumn, i]);
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
                currentMatches.Add(candyPosition[i, boardRow]);
                candyPosition[i, boardRow].GetComponent<Candy>().isMatched = true;
            }
        }
    }
    public void GetStripeChain()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            GameObject candy = currentMatches[i];
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
}

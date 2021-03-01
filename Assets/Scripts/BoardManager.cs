using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public int column;
    public int row;

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
        ScanForMatches();
    }

    public void CreateBoard()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                int random = Random.Range(0, candyType.Count);
                Vector2 tempPosition = new Vector2(i, j);
                GameObject newCandy = Instantiate(candyType[random], tempPosition, Quaternion.identity);

                newCandy.GetComponent<Candy>().atRow = j;
                newCandy.GetComponent<Candy>().atColumn = i;
                newCandy.transform.parent = this.transform;
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
    }
}

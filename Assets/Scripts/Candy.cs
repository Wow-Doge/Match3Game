using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public int atColumn;
    public int atRow;
    public bool isMatched = false;
    public int previousColumn;
    public int previousRow;

    private Vector2 firstTouch;
    private Vector2 finalTouch;

    private float distanceX;
    private float distanceY;
    private float minDistance = 0.5f;

    private GameObject otherCandy;
    private void Start()
    {
        previousColumn = atColumn;
        previousRow = atRow;
    }

    private void Update()
    {
        WhenMatches();
    }
    private void OnMouseDown()
    {
        firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateDistance();
    }

    private void CalculateDistance()
    {
        distanceX = Mathf.Abs(firstTouch.x - finalTouch.x);
        distanceY = Mathf.Abs(firstTouch.y - finalTouch.y);

        if (distanceX > minDistance || distanceY > minDistance)
        {
            MoveCandy();
        }
    }

    private void MoveCandy()
    {
        if (distanceX > distanceY)
        {
            //move horizontal
            if (firstTouch.x > finalTouch.x && atColumn > 0)
                MoveLeft();
            if (firstTouch.x < finalTouch.x && atColumn < BoardManager.Instance.column - 1)
                MoveRight();
        }
        else
        {
            //move vertical
            if (firstTouch.y > finalTouch.y && atRow > 0)
                MoveDown();
            if (firstTouch.y < finalTouch.y && atRow < BoardManager.Instance.row - 1)
                MoveUp();
        }
    }

    private void MoveLeft()
    {
        otherCandy = BoardManager.Instance.candyPosition[atColumn - 1, atRow];
        otherCandy.GetComponent<Candy>().atColumn += 1;
        this.atColumn -= 1;

        GetPosition();
        otherCandy.GetComponent<Candy>().GetPosition();
        BoardManager.Instance.ScanForMatches();
        if (isMatched)
        {

        }
        else
        {
            StartCoroutine(NotMatch(otherCandy));
        }
    }
    private void MoveRight()
    {
        otherCandy = BoardManager.Instance.candyPosition[atColumn + 1, atRow];
        otherCandy.GetComponent<Candy>().atColumn -= 1;
        this.atColumn += 1;

        GetPosition();
        otherCandy.GetComponent<Candy>().GetPosition();
        BoardManager.Instance.ScanForMatches();
        if (isMatched)
        {

        }
        else
        {
            StartCoroutine(NotMatch(otherCandy));
        }
    }
    private void MoveUp()
    {
        otherCandy = BoardManager.Instance.candyPosition[atColumn, atRow + 1];
        otherCandy.GetComponent<Candy>().atRow -= 1;
        this.atRow += 1;

        GetPosition();
        otherCandy.GetComponent<Candy>().GetPosition();
        BoardManager.Instance.ScanForMatches();
        if (isMatched)
        {

        }
        else
        {
            StartCoroutine(NotMatch(otherCandy));
        }
    }
    private void MoveDown()
    {
        otherCandy = BoardManager.Instance.candyPosition[atColumn, atRow - 1];
        otherCandy.GetComponent<Candy>().atRow += 1;
        this.atRow -= 1;

        GetPosition();
        otherCandy.GetComponent<Candy>().GetPosition();
        BoardManager.Instance.ScanForMatches();
        if (isMatched)
        {

        }
        else
        {
            StartCoroutine(NotMatch(otherCandy));
        }
    }
    public void GetPosition()
    {
        BoardManager.Instance.candyPosition[atColumn, atRow] = this.gameObject;
        transform.position = new Vector2(atColumn, atRow);
    }

    public IEnumerator NotMatch(GameObject otherCandy)
    {
        yield return new WaitForSeconds(0.5f);
        if (otherCandy != null)
        {
            if (!isMatched && !otherCandy.GetComponent<Candy>().isMatched)
            {
                otherCandy.GetComponent<Candy>().atRow = atRow;
                otherCandy.GetComponent<Candy>().atColumn = atColumn;
                atColumn = previousColumn;
                atRow = previousRow;
                GetPosition();
                otherCandy.GetComponent<Candy>().GetPosition();
            }
        }
    }

    public void WhenMatches() 
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, 0.2f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public int atColumn;
    public int atRow;
    public bool isMatched = false;

    private Vector2 firstTouch;
    private Vector2 finalTouch;

    private float distanceX;
    private float distanceY;
    private float minDistance = 0.5f;

    private GameObject otherCandy;
    private void Start()
    {
        
    }

    private void Update()
    {
        ChangeColor();
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
            if (firstTouch.x > finalTouch.x && atColumn > 0)
                MoveLeft();
            if (firstTouch.x < finalTouch.x && atColumn < BoardManager.Instance.column - 1)
                MoveRight();
        }
        else
        {
            if (firstTouch.y > finalTouch.y && atRow > 0)
                MoveDown();
            if (firstTouch.y < finalTouch.y && atRow < BoardManager.Instance.row - 1)
                MoveUp();
        }
    }

    private void MoveLeft()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn - 1, atRow];
        otherCandy.GetComponent<Candy>().atColumn += 1;
        this.atColumn -= 1;
        Vector2 target = new Vector2(atColumn, atRow);

        StartCoroutine(MoveObject(gameObject, current, target, 0.4f));
        StartCoroutine(MoveObject(otherCandy, target, current, 0.4f));

        if (isMatched)
        {
            
        }
        else
        {
            StartCoroutine(MoveBack(otherCandy));
        }
    }
    private void MoveRight()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn + 1, atRow];
        otherCandy.GetComponent<Candy>().atColumn -= 1;
        this.atColumn += 1;
        Vector2 target = new Vector2(atColumn, atRow);

        StartCoroutine(MoveObject(gameObject, current, target, 0.4f));
        StartCoroutine(MoveObject(otherCandy, target, current, 0.4f));

        if (isMatched)
        {
            
        }
        else
        {
            StartCoroutine(MoveBack(otherCandy));
        }
    }
    private void MoveUp()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn, atRow + 1];
        otherCandy.GetComponent<Candy>().atRow -= 1;
        this.atRow += 1;
        Vector2 target = new Vector2(atColumn, atRow);

        StartCoroutine(MoveObject(gameObject, current, target, 0.4f));
        StartCoroutine(MoveObject(otherCandy, target, current, 0.4f));

        if (isMatched)
        {
            
        }
        else
        {
            StartCoroutine(MoveBack(otherCandy));
        }
    }
    private void MoveDown()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn, atRow - 1];
        otherCandy.GetComponent<Candy>().atRow += 1;
        this.atRow -= 1;
        Vector2 target = new Vector2(atColumn, atRow);

        StartCoroutine(MoveObject(gameObject, current, target, 0.4f));
        StartCoroutine(MoveObject(otherCandy, target, current, 0.4f));

        if (isMatched)
        {
            
        }
        else
        {
            StartCoroutine(MoveBack(otherCandy));
        }
    }
    public void GetPosition()
    {
        BoardManager.Instance.candyPosition[atColumn, atRow] = this.gameObject;
        transform.position = new Vector2(atColumn, atRow);
    }

    public IEnumerator MoveObject(GameObject candyObject, Vector2 current, Vector2 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            candyObject.transform.position = Vector2.Lerp(current, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        candyObject.GetComponent<Candy>().GetPosition();
        BoardManager.Instance.ScanForMatches();
    }

    public IEnumerator MoveBack(GameObject otherCandy)
    {
        yield return new WaitForSeconds(0.5f);
        if (otherCandy != null)
        {
            if (!isMatched && !otherCandy.GetComponent<Candy>().isMatched)
            {
                Vector2 target = new Vector2(atColumn, atRow);
                int targetRow = otherCandy.GetComponent<Candy>().atRow;
                int targetColumn = otherCandy.GetComponent<Candy>().atColumn;
                Vector2 current = new Vector2(targetColumn, targetRow);
                otherCandy.GetComponent<Candy>().atRow = atRow;
                otherCandy.GetComponent<Candy>().atColumn = atColumn;
                atColumn = targetColumn;
                atRow = targetRow;

                StartCoroutine(MoveObject(gameObject, target, current, 0.4f));
                StartCoroutine(MoveObject(otherCandy, current, target, 0.4f));
            }
        }
    }

    public void ChangeColor() 
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, 0.2f);
        }
    }
}

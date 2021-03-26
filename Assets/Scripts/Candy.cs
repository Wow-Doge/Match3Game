﻿using System.Collections;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public int atColumn;
    public int atRow;
    public bool isMatched = false;

    public bool isColumnStripe;
    public bool isRowStripe;
    public bool isColorBomb;
    public bool isSquareBomb;
    public Sprite columnStripe;
    public Sprite rowStripe;
    public Sprite colorBomb;
    public Sprite squareBomb;

    private Vector2 firstTouch;
    private Vector2 finalTouch;

    private float distanceX;
    private float distanceY;
    private float minDistance = 0.5f;
    private float lerpTime = 0.2f;

    public GameObject otherCandy;

    public void Start()
    {
        isColumnStripe = false;
        isRowStripe = false;
        StartCoroutine(CollapseCandy());
    }

    private void OnMouseDown()
    {
        if (BoardManager.Instance.currentState == GameState.Idling)
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (BoardManager.Instance.currentState == GameState.Idling)
        {
            finalTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateDistance();
        }
    }

    //For testing purpose
    private void OnMouseOver()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    isColumnStripe = true;
        //    SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        //    mySprite.sprite = columnStripe;
        //}
        //if (Input.GetMouseButtonDown(2))
        //{
        //    isRowStripe = true;
        //    SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        //    mySprite.sprite = rowStripe;
        //}
        //if (Input.GetMouseButtonDown(2))
        //{
        //    isColorBomb = true;
        //    SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        //    mySprite.sprite = colorBomb;
        //    gameObject.name = "rainbow";
        //}
        //if (Input.GetMouseButtonDown(2))
        //{
        //    isSquareBomb = true;
        //    SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        //    mySprite.sprite = squareBomb;
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    isMatched = true;
        //    BoardManager.Instance.ScanBoard();
        //}
    }

    private void CalculateDistance()
    {
        distanceX = Mathf.Abs(firstTouch.x - finalTouch.x);
        distanceY = Mathf.Abs(firstTouch.y - finalTouch.y);

        if (distanceX > minDistance || distanceY > minDistance)
        {
            BoardManager.Instance.selectedCandy = this.gameObject;
            MoveCandy();
        }
    }
    private void MoveCandy()
    {
        BoardManager.Instance.currentState = GameState.Moving;
        if (distanceX > distanceY)
        {
            if (firstTouch.x > finalTouch.x && atColumn > 0)
                MoveLeft();
            if (firstTouch.x < finalTouch.x && atColumn < BoardManager.Instance.boardWidth - 1)
                MoveRight();
        }
        else
        {
            if (firstTouch.y > finalTouch.y && atRow > 0)
                MoveDown();
            if (firstTouch.y < finalTouch.y && atRow < BoardManager.Instance.boardHeight - 1)
                MoveUp();
        }
    }
    private void MoveLeft()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn - 1, atRow];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().atColumn += 1;
            this.atColumn -= 1;
            Vector2 target = new Vector2(atColumn, atRow);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BoardManager.Instance.currentState = GameState.Idling;
        }
    }
    private void MoveRight()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn + 1, atRow];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().atColumn -= 1;
            this.atColumn += 1;
            Vector2 target = new Vector2(atColumn, atRow);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BoardManager.Instance.currentState = GameState.Idling;
        }
    }
    private void MoveUp()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn, atRow + 1];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().atRow -= 1;
            this.atRow += 1;
            Vector2 target = new Vector2(atColumn, atRow);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BoardManager.Instance.currentState = GameState.Idling;
        }
    }
    private void MoveDown()
    {
        Vector2 current = new Vector2(atColumn, atRow);
        otherCandy = BoardManager.Instance.candyPosition[atColumn, atRow - 1];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().atRow += 1;
            this.atRow -= 1;
            Vector2 target = new Vector2(atColumn, atRow);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BoardManager.Instance.currentState = GameState.Idling;
        }
    }

    public void GetPosition()
    {
        BoardManager.Instance.candyPosition[atColumn, atRow] = this.gameObject;
    }
    public IEnumerator SwapObject(GameObject thisCandy, GameObject nextCandy, Vector2 current, Vector2 target, float overTime)
    {
        thisCandy.GetComponent<Candy>().GetPosition();
        nextCandy.GetComponent<Candy>().GetPosition();
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            thisCandy.transform.position = Vector2.Lerp(current, target, (Time.time - startTime) / overTime);
            nextCandy.transform.position = Vector2.Lerp(target, current, (Time.time - startTime) / overTime);
            yield return null;
        }
        thisCandy.transform.position = target;
        nextCandy.transform.position = current;

        if (thisCandy.GetComponent<Candy>().isColorBomb)
        {
            BoardManager.Instance.GetSameColorCandies(otherCandy);
            thisCandy.GetComponent<Candy>().isMatched = true;
        }
        else if (otherCandy.GetComponent<Candy>().isColorBomb)
        {
            BoardManager.Instance.GetSameColorCandies(thisCandy);
            otherCandy.GetComponent<Candy>().isMatched = true;
        }
        BoardManager.Instance.ScanBoard();
    }
    public IEnumerator SwapObjectBack(GameObject thisCandy, GameObject nextCandy, Vector2 current, Vector2 target, float overTime)
    {
        thisCandy.GetComponent<Candy>().GetPosition();
        nextCandy.GetComponent<Candy>().GetPosition();
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            thisCandy.transform.position = Vector2.Lerp(current, target, (Time.time - startTime) / overTime);
            nextCandy.transform.position = Vector2.Lerp(target, current, (Time.time - startTime) / overTime);
            yield return null;
        }
        thisCandy.transform.position = target;
        nextCandy.transform.position = current;
        yield return new WaitForSeconds(lerpTime);
        BoardManager.Instance.currentState = GameState.Idling;
    }
    public IEnumerator MoveBack(GameObject otherCandy)
    {
        yield return new WaitForSeconds(lerpTime);
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

                StartCoroutine(SwapObjectBack(gameObject, otherCandy, target, current, lerpTime));
            }
        }
    }

    public IEnumerator CollapseCandy()
    {
        float startTime = Time.time;
        if (this.gameObject != null)
        {
            while (Time.time < startTime + lerpTime)
            {
                transform.position = Vector2.Lerp(transform.position, new Vector2(atColumn, atRow), ((Time.time - startTime) / lerpTime));
                yield return null;
            }
            this.gameObject.transform.position = new Vector2(atColumn, atRow);
        }
    }
    public void ColumnStripeCandy()
    {
        isColumnStripe = true;
        isRowStripe = false;
        isColorBomb = false;
        isSquareBomb = false;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.sprite = columnStripe;
    }
    public void RowStripeCandy()
    {
        isRowStripe = true;
        isColumnStripe = false;
        isColorBomb = false;
        isSquareBomb = false;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.sprite = rowStripe;
    }
    public void ColorBombCandy()
    {
        isColumnStripe = false;
        isRowStripe = false;
        isColorBomb = true;
        isSquareBomb = false;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        gameObject.name = "rainbow";
        mySprite.sprite = colorBomb;
    }
    public void SquareBombCandy()
    {
        isColumnStripe = false;
        isRowStripe = false;
        isColorBomb = false;
        isSquareBomb = true;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.sprite = squareBomb;
    }
}

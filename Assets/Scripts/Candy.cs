using System.Collections;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public int column;
    public int row;
    public bool isMatched = false;

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
    public enum SpecialCandy
    {
        None,
        ColumnStripe,
        RowStripe,
        SquareBomb,
        ColorBomb
    }
    public SpecialCandy special;

    public void Start()
    {
        special = SpecialCandy.None;
        StartCoroutine(CollapseCandy());
    }

    private void OnMouseDown()
    {
        if (BattleSystem.Instance.battleState == BattleState.PLAYERTURN)
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (BattleSystem.Instance.battleState == BattleState.PLAYERTURN)
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
        //    BoardManager.Instance.FindMatches();
        //}
    }

    private void CalculateDistance()
    {
        distanceX = Mathf.Abs(firstTouch.x - finalTouch.x);
        distanceY = Mathf.Abs(firstTouch.y - finalTouch.y);

        if (distanceX > minDistance || distanceY > minDistance)
        {
            BoardManager.Instance.selectCandy = this.gameObject;
            MoveCandy();
        }
    }
    private void MoveCandy()
    {
        BattleSystem.Instance.battleState = BattleState.PLAYERMOVING;
        if (distanceX > distanceY)
        {
            if (firstTouch.x > finalTouch.x && column > 0)
                MoveLeft();
            if (firstTouch.x < finalTouch.x && column < BoardManager.Instance.width - 1)
                MoveRight();
        }
        else
        {
            if (firstTouch.y > finalTouch.y && row > 0)
                MoveDown();
            if (firstTouch.y < finalTouch.y && row < BoardManager.Instance.height - 1)
                MoveUp();
        }
    }
    private void MoveLeft()
    {
        Vector2 current = new Vector2(column, row);
        otherCandy = BoardManager.Instance.candyPos[column - 1, row];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().column += 1;
            this.column -= 1;
            Vector2 target = new Vector2(column, row);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BattleSystem.Instance.battleState = BattleState.PLAYERTURN;
        }
    }
    private void MoveRight()
    {
        Vector2 current = new Vector2(column, row);
        otherCandy = BoardManager.Instance.candyPos[column + 1, row];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().column -= 1;
            this.column += 1;
            Vector2 target = new Vector2(column, row);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BattleSystem.Instance.battleState = BattleState.PLAYERTURN;
        }
    }
    private void MoveUp()
    {
        Vector2 current = new Vector2(column, row);
        otherCandy = BoardManager.Instance.candyPos[column, row + 1];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().row -= 1;
            this.row += 1;
            Vector2 target = new Vector2(column, row);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BattleSystem.Instance.battleState = BattleState.PLAYERTURN;
        }
    }
    private void MoveDown()
    {
        Vector2 current = new Vector2(column, row);
        otherCandy = BoardManager.Instance.candyPos[column, row - 1];
        if (otherCandy != null)
        {
            otherCandy.GetComponent<Candy>().row += 1;
            this.row -= 1;
            Vector2 target = new Vector2(column, row);

            StartCoroutine(SwapObject(gameObject, otherCandy, current, target, lerpTime));

            if (!isMatched)
            {
                StartCoroutine(MoveBack(otherCandy));
            }
        }
        else
        {
            BattleSystem.Instance.battleState = BattleState.PLAYERTURN;
        }
    }

    public void GetPos()
    {
        BoardManager.Instance.candyPos[column, row] = this.gameObject;
    }
    public IEnumerator SwapObject(GameObject thisCandy, GameObject nextCandy, Vector2 current, Vector2 target, float overTime)
    {
        thisCandy.GetComponent<Candy>().GetPos();
        nextCandy.GetComponent<Candy>().GetPos();
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            thisCandy.transform.position = Vector2.Lerp(current, target, (Time.time - startTime) / overTime);
            nextCandy.transform.position = Vector2.Lerp(target, current, (Time.time - startTime) / overTime);
            yield return null;
        }
        thisCandy.transform.position = target;
        nextCandy.transform.position = current;

        if (thisCandy.GetComponent<Candy>().special == SpecialCandy.ColorBomb)
        {
            BoardManager.Instance.GetColor(otherCandy);
            thisCandy.GetComponent<Candy>().isMatched = true;
        }
        else if (otherCandy.GetComponent<Candy>().special == SpecialCandy.ColorBomb)
        {
            BoardManager.Instance.GetColor(thisCandy);
            otherCandy.GetComponent<Candy>().isMatched = true;
        }
        BoardManager.Instance.FindMatches();
    }
    public IEnumerator SwapObjectBack(GameObject thisCandy, GameObject nextCandy, Vector2 current, Vector2 target, float overTime)
    {
        thisCandy.GetComponent<Candy>().GetPos();
        nextCandy.GetComponent<Candy>().GetPos();
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
        BattleSystem.Instance.battleState = BattleState.PLAYERTURN;
    }
    public IEnumerator MoveBack(GameObject otherCandy)
    {
        yield return new WaitForSeconds(lerpTime);
        if (otherCandy != null)
        {
            if (!isMatched && !otherCandy.GetComponent<Candy>().isMatched)
            {
                Vector2 target = new Vector2(column, row);
                int targetRow = otherCandy.GetComponent<Candy>().row;
                int targetColumn = otherCandy.GetComponent<Candy>().column;
                Vector2 current = new Vector2(targetColumn, targetRow);
                otherCandy.GetComponent<Candy>().row = row;
                otherCandy.GetComponent<Candy>().column = column;
                column = targetColumn;
                row = targetRow;

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
                transform.position = Vector2.Lerp(transform.position, new Vector2(column, row), ((Time.time - startTime) / lerpTime));
                yield return null;
            }
            this.gameObject.transform.position = new Vector2(column, row);
        }
    }
    public void ColumnStripe()
    {
        isMatched = false;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.sprite = columnStripe;
        special = SpecialCandy.ColumnStripe;
    }
    public void RowStripe()
    {
        isMatched = false;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.sprite = rowStripe;
        special = SpecialCandy.RowStripe;
    }
    public void ColorBomb()
    {
        isMatched = false;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        gameObject.name = "rainbow";
        mySprite.sprite = colorBomb;
        special = SpecialCandy.ColorBomb;
    }
    public void SquareBomb()
    {
        isMatched = false;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.sprite = squareBomb;
        special = SpecialCandy.SquareBomb;
    }
}

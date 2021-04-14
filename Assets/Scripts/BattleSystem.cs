using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum BattleState
{
    START,
    PLAYERTURN,
    PLAYERMOVING,
    ENEMYTURN,
    WIN,
    LOST
}
public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;

    public BattleState battleState;

    public List<GameObject> charList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();

    public Transform charPos;
    public Transform enemyPos;

    public List<GameObject> charBattle = new List<GameObject>();
    public List<GameObject> enemyBattle = new List<GameObject>();

    public bool isContinue_1 = false;
    public bool isContinue_2 = false;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        battleState = BattleState.START;
        StartCoroutine(StartBattle());

        BoardManager.Instance.OnTurnEnd += NextTurn;
    }

    private void NextTurn(object sender, System.EventArgs e)
    {
        StartCoroutine(NextTurn_());
    }
    private IEnumerator NextTurn_()
    {
        PlayerAttack();

        yield return new WaitForSeconds(1f);
        foreach (var enemy in enemyBattle)
        {
            enemy.GetComponent<EnemyManager>().DecreaseCharge();
        }
        if (enemyBattle.Any(enemy => enemy.GetComponent<EnemyManager>().currentCharge <= 0))
        {
            isContinue_1 = false;
            StartCoroutine(EnemyTurn());
            yield return new WaitUntil(() => isContinue_1);
            Debug.Log("Enemy finish turn");
        }
        battleState = BattleState.PLAYERTURN;
    }

    public void PlayerAttack()
    {
        int amount = 0;
        foreach (var kvp in BoardManager.Instance.dict)
        {
            foreach (var thisChar in charBattle)
            {
                string color = thisChar.GetComponent<CharacterManager>().color.ToString();
                int damage = thisChar.GetComponent<CharacterManager>().damage;
                if (color == kvp.Key)
                {
                    amount += kvp.Value.Count * damage;
                }
            }
        }
        foreach (var enemy in enemyBattle)
        {
            enemy.GetComponent<EnemyManager>().TakeDamage(amount);
        }
    }

    private IEnumerator StartBattle()
    {
        SpawnCharacters();
        SpawnEnemies();
        yield return new WaitForSeconds(2f);
        battleState = BattleState.PLAYERTURN;
    }

    public void SpawnCharacters()
    {
        for (int i = 0; i < charList.Count; i++)
        {
            GameObject a = Instantiate(charList[i], charList[i].transform.position, Quaternion.identity);
            a.transform.SetParent(charPos, false);
            a.transform.localPosition = new Vector2(0, -150 + 150 * i);
            charBattle.Add(a);
        }
    }
    public void SpawnEnemies()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject b = Instantiate(enemyList[i], enemyList[i].transform.position, Quaternion.identity);
            b.transform.SetParent(enemyPos, false);
            b.transform.localPosition = new Vector2(0, -150 + 150 * i);
            enemyBattle.Add(b);
        }

        foreach (var enemy in enemyBattle)
        {
            enemy.GetComponent<EnemyManager>().HideSelectCircle();
        }
        enemyBattle.ElementAt(0).GetComponent<EnemyManager>().ShowSelectCircle();
    }

    public IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy turn");
        BattleSystem.Instance.battleState = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyAttack());
        yield return new WaitUntil(() => isContinue_2);
        ResetCharge();
        yield return new WaitForSeconds(1f);
        isContinue_1 = true;
    }

    public void ResetCharge()
    {
        foreach (var enemy in enemyBattle)
        {
            int currentCharge = enemy.GetComponent<EnemyManager>().currentCharge;
            if (currentCharge < 1)
            {
                enemy.GetComponent<EnemyManager>().ResetCharge();
            }
        }
        Debug.Log("reset charge");
    }

    public IEnumerator EnemyAttack()
    {
        isContinue_2 = false;
        foreach (var enemy in enemyBattle)
        {
            int amount = enemy.GetComponent<EnemyManager>().damage;
            GameObject target = charBattle.ElementAt(Random.Range(0, charBattle.Count));
            target.GetComponent<CharacterManager>().TakeDamage(amount);
            Debug.Log("Enemy attack: " + target.name + ", damage: " + amount);
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("enemy finish attack");
        isContinue_2 = true;
    }
}

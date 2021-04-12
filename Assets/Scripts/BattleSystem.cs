using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public List<GameObject> characterList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();

    public Transform characterPlace;
    public Transform enemyPlace;

    public List<GameObject> charBattle = new List<GameObject>();
    public List<GameObject> enemyBattle = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        battleState = BattleState.START;
        StartCoroutine(StartBattle());

        BoardManager.Instance.OnTurnEnd += Enemy_TakeDamage;
    }

    private IEnumerator StartBattle()
    {
        SpawnCharacters();
        SpawnEnemies();
        yield return new WaitForSeconds(2f);
        battleState = BattleState.PLAYERTURN;
    }

    private void Enemy_TakeDamage(Dictionary<string, List<GameObject>> dict)
    {
        int amount = 0;
        foreach (var kvp in dict)
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

    public void SpawnCharacters()
    {
        foreach (GameObject character in characterList)
        {
            GameObject a = Instantiate(character, gameObject.transform.position, Quaternion.identity);
            a.transform.SetParent(characterPlace, false);
            charBattle.Add(a);
        }
    }
    public void SpawnEnemies()
    {
        foreach (GameObject enemy in enemyList)
        {
            GameObject b = Instantiate(enemy, gameObject.transform.position, Quaternion.identity);
            b.transform.SetParent(enemyPlace, false);
            enemyBattle.Add(b);
        }
    }
}

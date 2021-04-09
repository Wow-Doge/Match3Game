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
    public BattleState battleState;

    public GameObject character;
    public GameObject enemy;

    public Transform characterPlace;
    public Transform enemyPlace;

    public GameObject char1;
    public GameObject enemy1;

    public static BattleSystem Instance;
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
        char1 = Instantiate(character, gameObject.transform.position, Quaternion.identity);
        enemy1 = Instantiate(enemy, gameObject.transform.position, Quaternion.identity);

        char1.transform.SetParent(characterPlace, false);
        enemy1.transform.SetParent(enemyPlace, false);
        yield return new WaitForSeconds(2f);
        battleState = BattleState.PLAYERTURN;
    }

    private void Enemy_TakeDamage(Dictionary<string, List<GameObject>> dict)
    {
        int damage = char1.GetComponent<CharacterManager>().damage;
        string color = char1.GetComponent<CharacterManager>().color.ToString();
        int amount = 0;
        foreach (var kvp in dict)
        {
            //Debug.Log("color: " + kvp.Key + " / " + "Num: " + kvp.Value.Count + " = " + kvp.Value.Count * damage);
            if (color == kvp.Key)
            {
                damage *= 2;
            }
            amount += kvp.Value.Count * damage;
        }
        enemy1.GetComponent<EnemyManager>().TakeDamage(amount);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy(GameObject Enemy, Vector3 Position, Quaternion Rotation)
    {
        GameObject NewEnemy = Instantiate(Enemy, Position, Rotation);
        Debug.Log(Player.transform.position.x.ToString() + " --- " + NewEnemy.transform.position.x.ToString());
        if (Enemy.GetComponent<EnemyMovement>()) Enemy.GetComponent<EnemyMovement>().SetKirby(Player);

        if (Player.transform.position.x < NewEnemy.transform.position.x)
        {
            NewEnemy.GetComponent<EnemyMovement>().SetDirection(-1);
            //Debug.Log("1");
        }
        else
        {
            NewEnemy.GetComponent<EnemyMovement>().SetDirection(1);
            //Debug.Log("-1");
        }
    }
}

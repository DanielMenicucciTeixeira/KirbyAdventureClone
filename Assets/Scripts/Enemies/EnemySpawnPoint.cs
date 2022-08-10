using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemySpawnPoint : MonoBehaviour
{
    public GameObject Enemy;
    public float SpawnRadius;
    EnemySpawnManager SpawnManager;
    bool InRange = false;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(SpawnRadius * 2, GetComponent<BoxCollider2D>().size.y);
        GetComponent<BoxCollider2D>().isTrigger = true;
        if (GetComponent<SpriteRenderer>()) Destroy(GetComponent<SpriteRenderer>());
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnManager = FindObjectOfType<EnemySpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!InRange)
        {
           //Debug.Log("HelloThere!");
            if (collision.gameObject.GetComponent<PlayerController>())
            {
               // Debug.Log("General Kenobi!");
                SpawnManager.SpawnEnemy(Enemy, transform.position, transform.rotation);

                InRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            InRange = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(SpawnRadius, 10, 1));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BaseProjectile : MonoBehaviour
{
    Rigidbody2D Body;

    public float Speed;
    public float LifeTime;
    protected float TimeAlive = 0;


    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeAlive > LifeTime)
        {
            SelfDestroy();
        }
        else
        {
            TimeAlive += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision);
    }

    protected virtual void SelfDestroy()
    {
        Destroy(gameObject);
    }

    protected virtual void OnHit(Collision2D collision)
    {
        if(!collision.gameObject.GetComponent<PlayerController>())
        {
            if (collision.gameObject.GetComponent<EnemyCharacter>()) Destroy(collision.gameObject);
            SelfDestroy();
        }
    }
}

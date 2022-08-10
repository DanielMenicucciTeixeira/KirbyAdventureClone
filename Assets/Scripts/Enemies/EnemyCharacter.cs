using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDamageType
{
    NORMAL,
    ELECTRIC,
    FIRE
}

public class EnemyCharacter : MonoBehaviour
{
    public ePowerType Power;
    public eDamageType DamageType = eDamageType.NORMAL;
    public float MaxDistance = 2.0f;
    public int Damage = 1;


    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnGUI()
    {
        GUILayout.TextArea(GetComponent<SpriteRenderer>().isVisible.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs((FindObjectOfType<PlayerController>().transform.position - transform.position).x) > (MaxDistance)) Destroy(gameObject);
        else GetComponent<EnemyMovement>().RunMovement();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>())
        {
            if(collision.gameObject.GetComponent<PlayerController>().IsDeadly())
            {
                Destroy(gameObject);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(Damage, DamageType, gameObject);
            }
        }
    }
}

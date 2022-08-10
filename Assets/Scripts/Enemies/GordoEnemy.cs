using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GordoEnemy : EnemyCharacter
{ 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs((FindObjectOfType<PlayerController>().transform.position - transform.position).x) > (MaxDistance))
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<EnemyMovement>().RunMovement();
        }
    }
}

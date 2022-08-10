using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhustAttackProjectile : BaseProjectile
{
    public float SecondsBeforDestroy;
    float SecondsWaited = 0;

    protected override void SelfDestroy()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        if (SecondsWaited >= SecondsBeforDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            SecondsWaited += Time.deltaTime;
        }
    }

}

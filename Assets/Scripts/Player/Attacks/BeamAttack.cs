using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class BeamAttack : MonoBehaviour
{
    bool Active = false;
    public float RotationSpeed = 75.0f;
    public float MinRotation = 0.0f;
    public float StartAngle = 75.0f;
    float CurrentAngle;

    public void SetActive(bool IsActive)
    {
        GetComponent<SpriteRenderer>().forceRenderingOff = !IsActive;
        Active = IsActive;
    }

    public bool IsActive()
    {
        return Active;
    }

    public void SetFlip(bool Flip)
    {
        GetComponent<SpriteRenderer>().flipX = Flip;
    }

    void Rotate()
    {
        int FlipCorrection = 1;
        if (GetComponent<SpriteRenderer>().flipX) FlipCorrection = -1;

        if ((FlipCorrection == 1 && CurrentAngle < MinRotation) || (FlipCorrection == -1 && CurrentAngle > -MinRotation))
        {
            gameObject.transform.rotation = Quaternion.AngleAxis(StartAngle, Vector3.forward);
            CurrentAngle = StartAngle;
        }
        else
        {
            CurrentAngle += FlipCorrection * RotationSpeed * Time.deltaTime;
            gameObject.transform.rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward);
        }
    }
        

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.rotation = Quaternion.AngleAxis(StartAngle, Vector3.forward);
        CurrentAngle = StartAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if(Active)
        {
            Rotate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Active && collision.gameObject.GetComponent<EnemyCharacter>())
        {
            Destroy(collision.gameObject);
        }
    }
}

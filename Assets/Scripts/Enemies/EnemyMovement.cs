using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eEnemyMovementType
{
    WALK,
    FLY,
    FOLLOW,
    HOP,
    SWALLOWED,
    NUMBER_OF_MOVEMENTS
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D Body;

    public eEnemyMovementType MovementType;
    GameObject Kirby;
    public float Speed = 0.8f;
    public float FallSpeed = 1.5f;
    public float InitialHopSpeed = 1.0f;
    public float AirbornTime = 1.0f;
    public float HopHorizonttalSpeed = 1.0f;
    public float HopDelay = 0.8f;
    float HopTimer = 0.0f;
    float SlowFallDecrement;
    int Direction;

    bool Grounded;
    public Vector2 GroundCheckBox = new Vector2(0.05f, 0.01f);
    public LayerMask GroundMask;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        SlowFallDecrement =  (InitialHopSpeed)/ AirbornTime;
        HopTimer = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        Kirby = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RunMovement()
    {
        GroundCheck();

        switch (MovementType)
        {
            case eEnemyMovementType.WALK:
                Walk();
                break;
            case eEnemyMovementType.FOLLOW:
                Follow();
                break;
            case eEnemyMovementType.HOP:
                Hop();
                break;
            case eEnemyMovementType.FLY:
                Fly();
                break;
            case eEnemyMovementType.SWALLOWED:
                break;
            default:
                break;
        }

        SetSpriteFlip();
    }

    public void SetKirby(GameObject Player)
    {
        Kirby = Player;
    }

    void Walk()
    {
        if (Grounded) Body.velocity = new Vector2(Speed * Direction, Body.velocity.y);
        else Fall();
    }

    void Follow()
    {

    }

    void Fly()
    {

    }

    void Hop()
    {
        if (Kirby.transform.position.x < transform.position.x)
        {
            SetDirection(-1);
        }
        else
        {
            SetDirection(1);
        }

        if (Grounded && HopTimer > HopDelay)
        {
            Body.velocity = Direction * Vector2.right * HopHorizonttalSpeed + Vector2.up * InitialHopSpeed;
            Grounded = false;
            HopTimer = 0;
            GetComponent<Animator>().SetBool("SpringMode", false);
        }
        else if(!Grounded)
        {
            if (Body.velocity.y > 0) Body.velocity = Direction * Vector2.right * HopHorizonttalSpeed + Vector2.up * (Body.velocity.y - (SlowFallDecrement * Time.deltaTime));
            else Body.velocity = Direction * Vector2.right * HopHorizonttalSpeed + Vector2.up * -FallSpeed;
            GetComponent<Animator>().SetBool("SpringMode", false);
        }
        else
        {
            HopTimer += Time.deltaTime;
            GetComponent<Animator>().SetBool("SpringMode", true);
        }
    }

    void Fall()
    {
        Body.velocity = new Vector2(Body.velocity.x, -FallSpeed);
    }

    void GroundCheck()
    {
        if (Physics2D.OverlapBox(transform.position, GroundCheckBox, 0, GroundMask)) Grounded = true;
        else Grounded = false;
    }

    void SetSpriteFlip()
    {
        if (Direction > 0) GetComponent<SpriteRenderer>().flipX = true;
        else GetComponent<SpriteRenderer>().flipX = false;
    }

    public void SetDirection(int direction)
    {
        Direction = direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position, GroundCheckBox);
    }

    private void OnGUI()
    {
        GUILayout.TextArea(Direction.ToString());
    }
}

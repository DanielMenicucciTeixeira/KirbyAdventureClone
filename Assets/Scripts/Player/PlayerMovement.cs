using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    //public eBaseState MovementState;
    PlayerController Controller;

    Rigidbody2D Body;

    int FaceDirection = 1;

    public float Speed;
    public float RunSpeed;
    public float BreakForce;

    bool Grounded;

    bool Jumping = false;
    float SlowFallDecrement;
    public float MaxJumpTime;
    public float BaseFallSpeed;
    public float FloatFallSpeed;                                      
    public float InitialJumpSpeed;
    public BoxCollider2D GroundBox;

    public float BouceSpeed;
    public float BounceSpeedDecrease;
    bool Bounced = false;

    Vector2 HitRollDirection;
    public float HitRollSpeed;
    public float HitRotationSpeed;
    float DamageRotation = 0;
    public float HitArcHorizontalSpeed;
    public float HitArcVerticalSpeed;

    //How long the player has to fall to start a fall tackle
    public float FallTackleDelay;
    //How long the player has been falling
    float FallTimer = 0;

    public float FlySpeed;

    public float SlideTackleSpeed;

    //Size and position of the Groundcheck trigger area
    public Vector2 GroundCheckSize;
    public Transform GroundCheckCenter;
    public LayerMask GroundMask;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Controller = GetComponent<PlayerController>();
        SlowFallDecrement = InitialJumpSpeed/MaxJumpTime;
    }

    public void Ground(bool IsGrounded)
    {
        if(!Bounced || Body.velocity.y <= 0)
        {
            Grounded = IsGrounded;
        }
        else
        {
            Grounded = false;
        }

        if (Grounded)
        {
            Jumping = false;
            FallTimer = 0;
            if (Controller.State.Current != eControllState.BASE || Controller.State.Base != eBaseState.FALLING_TACKLE || Bounced)
            {
                Controller.State.Base = eBaseState.IDLE;
                Controller.State.Fat = eFatState.IDLE;
                if(Bounced) Controller.SetDeadly(false);
                Bounced = false;
            }
        }
        else if (Body.velocity.y <= 0)
        {
            if (Controller.State.Current != eControllState.BASE || Controller.State.Base != eBaseState.FALLING_TACKLE)
            {
                Controller.State.Base = eBaseState.FALLING;
                Controller.State.Fat = eFatState.FALLING;
            }
        }
    }

    public void Walk(float Direction)
    {
        if (Direction > 0)
        {
            Direction = 1.0f;
        }
        else if (Direction < 0)
        {
            Direction = -1.0f;
        }

        Body.velocity = new Vector2(Speed * Direction, Body.velocity.y);
       
        if(Direction >= 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            FaceDirection = 1;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
            FaceDirection = -1;
        }

        if(Grounded)
        {
            Controller.State.Base = eBaseState.WALKING;
            Controller.State.Fat = eFatState.WALKING;
        }
    }

    public void Run(float Direction)
    {
        if (Direction > 0)
        {
            Direction = 1.0f;
        }
        else if (Direction < 0)
        {
            Direction = -1.0f;
        }

        Body.velocity = new Vector2(RunSpeed * Direction, Body.velocity.y);

        if (Direction >= 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            FaceDirection = 1;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
            FaceDirection = -1;
        }

        if (Grounded)
        {
            Controller.State.Base = eBaseState.RUNNING;
        }
    }

    public void Stop()
    {
        Body.velocity = new Vector2(0, Body.velocity.y);
    }
    
    public void Break()
    {
        if (Body.velocity.x * FaceDirection > 0)
        {
            Body.velocity = Body.velocity - Vector2.right * BreakForce * FaceDirection * Time.deltaTime;
        }
        else
        {
            Body.velocity = new Vector2(0, Body.velocity.y);
        }

    }

    public void Jump()
    {
       if (Grounded && !Jumping)
         {
             Jumping = true;
             Ground(false);

            Body.velocity = new Vector2(Body.velocity.x, InitialJumpSpeed);
            Controller.State.Base = eBaseState.JUMPING;
            Controller.State.Fat = eFatState.JUMPING;
        }

       /* if(Grounded)
        {
            Body.AddForce(new Vector2(0, JumpForce));
            Ground(false);
            Controller.State.Base = eBaseState.JUMPING;
        }*/
    }

    public void HighJump()
    {
        if (Jumping && !Grounded)
        {
            if (Body.velocity.y > 0)
            {
                Body.velocity = Body.velocity - new Vector2(0, SlowFallDecrement * Time.deltaTime);
                Controller.State.Base = eBaseState.JUMPING;
                Controller.State.Fat = eFatState.JUMPING;
            }
            else
            {
                if(Controller.State.Current == eControllState.BASE)
                {
                    BaseFall();
                }
                else if(Controller.State.Current == eControllState.FAT)
                {
                    FatFall();
                }
            }
        }
    }
    
    public bool IsGrounded()
    {
        return Grounded;
    }

    public void GroundCheck()
    {
        if (!(Controller.State.Current == eControllState.BASE && Controller.State.Base == eBaseState.JUMPING) && !(Controller.State.Current == eControllState.FAT && Controller.State.Fat == eFatState.JUMPING))
        {
            Ground
                (
                Physics2D.OverlapArea
                (
                    new Vector2(GroundCheckCenter.position.x - 0.5f * GroundCheckSize.x,GroundCheckCenter.position.y - 0.5f * GroundCheckSize.y), 
                    new Vector2(GroundCheckCenter.position.x + 0.5f * GroundCheckSize.x, GroundCheckCenter.position.y - 0.51f * GroundCheckSize.y), 
                    GroundMask
                )
                );
        }
    }

    public void BaseFall()
    {
        if(!Bounced || Body.velocity.y <= 0)
        {

            Body.velocity = new Vector2(Body.velocity.x, -BaseFallSpeed);
            if (Controller.State.Base == eBaseState.FALLING)
            {
                if (FallTimer >= FallTackleDelay)
                {
                    Controller.SetDeadly(true);
                    Controller.State.Base = eBaseState.FALLING_TACKLE;
                }
                else
                {
                    FallTimer += Time.deltaTime;
                }
            }
            else
            {
                Controller.State.Base = eBaseState.FALLING;
            }
            Jumping = false;
        }
    }

    public void FatFall()
    {
        Body.velocity = new Vector2(Body.velocity.x, -BaseFallSpeed);
        Controller.State.Fat = eFatState.FALLING;
        Jumping = false;
    }

    public void FloatingFall()
    {
        Controller.State.Floating = eFloatingState.FLOATING;
        Body.velocity = new Vector2(Body.velocity.x, -FloatFallSpeed);
    }

    public void Fly()
    {
        Controller.State.Floating = eFloatingState.FLYING;
        Body.velocity = new Vector2(Body.velocity.x, FlySpeed);
    }

    public void SlideTackle()
    {
        Body.velocity = new Vector2(SlideTackleSpeed * FaceDirection, Body.velocity.y);
    }

    public void Bounce()
    {
        if(!Bounced)
        {

            Body.velocity = new Vector2(Body.velocity.x, BouceSpeed);
            Bounced = true;
        }
        else
        {
            Bounced = false;
        }
    }

    public void TackleFall()
    {
        if(!Bounced)
        {
            Body.velocity = new Vector2(Body.velocity.x, -BaseFallSpeed);
        }
        else if (Body.velocity.y > 0)
        {
            Body.velocity = Body.velocity - Vector2.up * BounceSpeedDecrease * Time.deltaTime;
        }
        else
        {
            Body.velocity = new Vector2(Body.velocity.x, -BaseFallSpeed);
        }

    }
    
    public void ResetBaseStateParameters()
    {
        FallTimer = 0;
        Bounced = false;
        Jumping = false;
    }

    public bool HasBounced()
    {
        return Bounced;
    }

    public int GetFaceDirection()
    {
        return FaceDirection;
    }

    public void SetHitRollDirection(Vector2 Direction)
    {
        HitRollDirection = Direction;
    }
    public void HitRoll()
    {
        DamageRotation += HitRollSpeed * Time.deltaTime;
        gameObject.transform.eulerAngles = Vector3.forward * DamageRotation;
        Body.velocity = HitRollDirection * HitRollSpeed;
    }

    public void SetDamageRotaion(float Rotation)
    {
        DamageRotation = Rotation;
    }

    public void HitArc()
    {
        Body.velocity = HitRollDirection * HitArcHorizontalSpeed + Vector2.up * HitArcVerticalSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(GroundCheckCenter.position, GroundCheckSize);
    }
}

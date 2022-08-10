using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ControllerStruct
{
    public string VerticalAxis, HorizontalAxis, Up, Down, Right, Left, A, B, Start, Select;
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //Component references, set in Awake
    ControllerStruct Controller;
    List<string> ButtonList;
    PlayerMovement Movement;
    PlayerAttack PowerManager;
    Animator PlayerAnimator;
    AnimatorControllers PlayerAnimatorControllers;
    Rigidbody2D Body;
    public GroundChecker GroundBox;
    public ControllStateIndex State;
    //--------------------------------------

    //Transition-control variables
    bool Deflating;
    bool Deadly = false;

    //How long the player must stay idle between each blink animation
    public float SecondsBetweenBlinks = 5.0f;

    //Duration of Slide Tackle
    public float SlideTackleDurantion;

    public float RunDoubleTapDelay;
    float RunDoubleTapTimer = 0;
    bool LastFrameRunTap;
    bool Running = false;

    //HP and Damage variables
    public int HP = 5;
    eDamageType DamageTaken = eDamageType.NORMAL;
    bool Invulnerable = false;
    float InvulnerableTimer = 0.0f;
    public float InvulnarableMaxTime = 0.2f;
    Quaternion IniialRotation;


    //Called Befor Start
    private void Awake()
    {
        ButtonList = new List<string>();
        SetComponentReferences();
        SetDefaultController();
        SetInitialStates();
        HP = 5;
        Invulnerable = false;
        InvulnerableTimer = 0.0f;
        Deadly = false;
        IniialRotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Movement.GroundCheck();
        if (State.Current != eControllState.ATTACK) PowerManager.BeamBlade.SetActive(false);

        switch (State.Current)
        {
            case eControllState.BASE:
                BaseControlls();
                break;
            case eControllState.CROUCHING:
                CrouchingControlls();
                break;
            case eControllState.FLOATING:
                FloatingControlls();
                break;
            case eControllState.ATTACK:
                AttackControlls();
                break;
            case eControllState.HIT:
                HitControlls();
                break;
            case eControllState.FAT:
                FatControls();
                break;
        }
    }

    /*
     * Adds a list of names of buttons, that must match the unity standard for the desired button and this exact order
     * Up/Down Axis, Left/Right Axis, Jump(A) buttom, Attack(B) button, Start button, Select Button
     */
    public void SetController(List<string> ButtonNames)
    {

        Controller.VerticalAxis = ButtonNames[0];//Up and down arrows in the original NES controller
        Controller.HorizontalAxis = ButtonNames[1];//Left and right arrows in the original NES controller
        Controller.A = ButtonNames[2];
        Controller.B = ButtonNames[3];
        Controller.Start = ButtonNames[4];
        Controller.Select = ButtonNames[5];
    }

    void CheckInvulnerable()
    {
        if(InvulnerableTimer > InvulnarableMaxTime)
        {
            Invulnerable = false;
            InvulnerableTimer = 0.0f;
        }
        else
        {
            InvulnerableTimer += Time.deltaTime;
        }
    }


    //Initial Setting Functions----------------------------------------------------------------------------------------
    void SetDefaultController()
    {
        ButtonList.Add("Vertical");
        ButtonList.Add("Horizontal");
        ButtonList.Add("Fire1");
        ButtonList.Add("Fire2");
        ButtonList.Add("Submit");
        ButtonList.Add("Cancel");

        SetController(ButtonList);
    }

    void SetInitialStates()
    {
        State.Base = eBaseState.IDLE;
        State.Crouching = eCrouchingState.IDLE;
        State.Floating = eFloatingState.FLOATING;
        State.Attack = eAttackState.SWALLOWING;
        State.Fat = eFatState.IDLE;
        State.Hit = eHitState.HIT;

        State.Current = eControllState.BASE;

        //Set the Animator controller to the Base State Controller
        PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Base;
    }

    void SetComponentReferences()
    {
        Movement = GetComponent<PlayerMovement>();
        PlayerAnimator = GetComponent<Animator>();
        PlayerAnimatorControllers = GetComponent<AnimatorControllers>();
        Body = GetComponent<Rigidbody2D>();
        PowerManager = GetComponent<PlayerAttack>();
    }
    //-------------------------------------------------------------------------------

    //Controll Functions-------------------------------------------------------------
    //The follwing functions controll the main character, and are called based on button input and the current state the player is in (State.Current)

    void BaseControlls()
    {
        if (Input.GetAxis(Controller.VerticalAxis) > 0)
        {
            ChangeToState(eControllState.FLOATING);
            return;
        }
        else if (Input.GetButtonDown(Controller.B) && (State.Base != eBaseState.FALLING_TACKLE || !Movement.HasBounced()))
        {
            ChangeToState(eControllState.ATTACK);
            return;
        }
        else if (State.Base == eBaseState.FALLING_TACKLE)
        {
            if (Movement.IsGrounded())
            {
                Movement.Bounce();
            }
            else
            {
                Movement.TackleFall();
            }
        }
        else if (Input.GetAxis(Controller.VerticalAxis) < 0 && Movement.IsGrounded())
        {
            ChangeToState(eControllState.CROUCHING);
            return;
        }
        else
        {
            if (Input.GetAxis(Controller.HorizontalAxis) != 0)
            {
                if (Running)
                {
                    Movement.Run(Input.GetAxis(Controller.HorizontalAxis));
                }
                else if (Movement.IsGrounded())
                {
                    if (!LastFrameRunTap && (State.Base == eBaseState.WALKING || State.Base == eBaseState.IDLE))
                    {
                       

                        if (RunDoubleTapTimer <= RunDoubleTapDelay)
                        {
                            Running = true;
                            Movement.Run(Input.GetAxis(Controller.HorizontalAxis));
                        }
                        else
                        {
                            Movement.Walk(Input.GetAxis(Controller.HorizontalAxis));
                        }
                    }
                    else
                    {
                        Movement.Walk(Input.GetAxis(Controller.HorizontalAxis));
                    }
                }
                else
                {
                    Movement.Walk(Input.GetAxis(Controller.HorizontalAxis));
                }

                RunDoubleTapTimer = 0;
                LastFrameRunTap = true;
            }
            else
            {
                Movement.Stop();
                RunDoubleTapTimer += Time.deltaTime;
                LastFrameRunTap = false;
                Running = false;
            }

            if (Input.GetButtonDown(Controller.A) && State.Base != eBaseState.JUMPING)
            {
                Movement.Jump();
            }
            else if (Input.GetButton(Controller.A) && State.Base == eBaseState.JUMPING)
            {
                Movement.HighJump();
            }
            else if (!Movement.IsGrounded() && State.Base != eBaseState.FALLING_TACKLE)
            {
                Movement.BaseFall();
            }

            //Set the MovementState in the AnimatorController to the State.Base of this frame
            PlayerAnimator.SetInteger("MovementState", State.Base.GetHashCode());

            //Check if Kriby should blink
            if (State.Base == eBaseState.IDLE && PlayerAnimator.GetFloat("BlinkTimer") < SecondsBetweenBlinks)
            {
                PlayerAnimator.SetFloat("BlinkTimer", PlayerAnimator.GetFloat("BlinkTimer") + Time.deltaTime);
            }
            else
            {
                PlayerAnimator.SetFloat("BlinkTimer", 0.0f);
            }
        }
    }

    void CrouchingControlls()
    {
        if (!Movement.IsGrounded())
        {
            ChangeToState(eControllState.BASE);
            return;
        }
        else if (State.Crouching == eCrouchingState.SLIDE_TACKLE)
        {
            /*if(SlideTackleTimer < SlideTackleDurantion)
            {
                SlideTackleTimer += Time.deltaTime;
            }
            else
            {
                SlideTackleTimer = 0;
                State.Crouching = eCrouchingState.IDLE;
            }*/
            Movement.Break();
            if (Body.velocity.x == 0)
            {
                SetDeadly(false);
                ChangeToState(eControllState.BASE);
                return;
            }

        }
        else if (Input.GetAxis(Controller.VerticalAxis) >= 0)
        {
            ChangeToState(eControllState.BASE);
            return;
        }
        else
        {
            if (Input.GetButtonDown(Controller.A) || Input.GetButtonDown(Controller.B))
            {
                State.Crouching = eCrouchingState.SLIDE_TACKLE;
                SetDeadly(true);
                Movement.SlideTackle();
            }
            else
            {
                Movement.Stop();
            }
        }

        //Set PlayerAnimator State
        PlayerAnimator.SetInteger("State", State.Crouching.GetHashCode());
    }

    void FloatingControlls()
    {
        if(Deflating)
        {
            ChangeToState(eControllState.BASE);
            return;
        }
        else if (Input.GetButtonDown(Controller.B) && !PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Inflate"))
        {
            State.Floating = eFloatingState.DEFLATING;
            Body.velocity = new Vector2(Body.velocity.x, 0);
            PowerManager.GhustAttack();
        }
        else
        {
            if (!Deflating && (Input.GetButton(Controller.A) || Input.GetAxis(Controller.VerticalAxis) > 0))
            {
                Movement.Fly();
            }
            else if (State.Floating != eFloatingState.DEFLATING)
            {
                Movement.FloatingFall();
            }

            if (Input.GetAxis(Controller.HorizontalAxis) != 0)
            {
                Movement.Walk(Input.GetAxis(Controller.HorizontalAxis));
            }
            else
            {
                Movement.Stop();
            }
        }

        //Set the MovementState in the AnimatorController to the State.Floating of this frame
        PlayerAnimator.SetInteger("State", State.Floating.GetHashCode());
    }

    void AttackControlls()
    {   //(!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Kirby_Inflate") || !PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Kirby_Spit"))
        if (!Input.GetButton(Controller.B) && !PowerManager.IsInLockedAnimation())
        {
            PowerManager.StopAttack();
            ChangeStateToBase();
            return;
        }
        else
        {
            Movement.Break();

            if (!Movement.IsGrounded())
            {
                Movement.BaseFall();
            }

            PowerManager.Attack();

            //Set the MovementState in the AnimatorController to the State.Floating of this frame
            PlayerAnimator.SetInteger("State", State.Attack.GetHashCode());
        }
    }

    void FatControls()
    {
        //if (GetComponent<PlayerMovement>().IsGrounded()) State.Fat = eFatState.IDLE;

        if(GetComponent<PlayerMovement>().IsGrounded() && Input.GetAxis(Controller.VerticalAxis) < 0)
        {
            PowerManager.AquirePower();
            State.Fat = eFatState.SWALLOWING;
        }
        else 
        {
            if (Input.GetButtonDown(Controller.B))
            {
                ChangeStateToAttack();
                return;
            }
            else if (Input.GetAxis(Controller.HorizontalAxis) != 0)
            {
                Movement.Walk(Input.GetAxis(Controller.HorizontalAxis));
            }
            else
            {
                Movement.Stop();
            }

            if (GetComponent<PlayerMovement>().IsGrounded() && Input.GetButtonDown(Controller.A) && State.Fat != eFatState.JUMPING)
            {
                Movement.Jump();
            }
            else if (Input.GetButton(Controller.A) && State.Fat == eFatState.JUMPING)
            {
                Movement.HighJump();
            }
            else if (!GetComponent<PlayerMovement>().IsGrounded())
            {
                Movement.FatFall();
            }
        }
        
        //Set the MovementState in the AnimatorController to the State.Fat of this frame
        PlayerAnimator.SetInteger("State", State.Fat.GetHashCode());
    }

    void HitControlls()
    {
        CheckInvulnerable();

        if(Invulnerable)
        {
            switch (DamageTaken)
            {
                case eDamageType.NORMAL:
                    Movement.HitRoll();
                    break;
                default:
                    Movement.HitArc();
                    break;
            }
        }
        else
        {
            gameObject.transform.rotation = IniialRotation;
            Movement.SetDamageRotaion(0.0f);
            Movement.Break();
            if (PowerManager.CurrentPower == ePowerType.SPIT) ChangeStateToFat();
            else ChangeStateToBase();
            return;
        }
        //Set the variables in the Animator Controller
        PlayerAnimator.SetInteger("State", DamageTaken.GetHashCode());
        if (PowerManager.CurrentPower == ePowerType.SPIT) PlayerAnimator.SetBool("IsFat", true);
        else PlayerAnimator.SetBool("IsFat", false);
    }
    //--------------------------------------------------------------------------------------------------------------------

    public void ChangeToState(eControllState NextState)
    {
        switch (NextState)
        {
            case eControllState.BASE:
                ChangeStateToBase();
                break;
            case eControllState.CROUCHING:
                ChangeStateToCrouching();
                break;
            case eControllState.FAT:
                ChangeStateToFat();
                break;
            case eControllState.FLOATING:
                ChangeStateToFloating();
                break;
            case eControllState.ATTACK:
                ChangeStateToAttack();
                break;
            case eControllState.HIT:
                ChangeStateToHit();
                break;
            default:
                break;
        }
    }

    void ChangeStateToBase()
    {
        if (State.Current != eControllState.FLOATING || (Deflating && !PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Kirby_Deflate")))
        {
            //Set the Animator controller to the Base State Controller
            PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Base;

            State.Current = eControllState.BASE;
            State.Base = eBaseState.IDLE;
            RunDoubleTapTimer = 0;
            Running = false;
            Movement.ResetBaseStateParameters();
            BaseControlls();
        }
    }

    void ChangeStateToFloating()
    {
        //Set the Animator controller to the Floating State Controller
        PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Floating;

        State.Current = eControllState.FLOATING;
        State.Floating = eFloatingState.FLOATING;
        Deflating = false;
        FloatingControlls();
    }

    void ChangeStateToCrouching()
    {
        //Set the Animator controller to the Crouching State Controller
        PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Crouching;

        State.Current = eControllState.CROUCHING;
        State.Crouching = eCrouchingState.IDLE;
        CrouchingControlls();
    }

    void ChangeStateToAttack()
    {
        //Set the Animator controller to the Attack State Controller
        PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Attack;

        State.Current = eControllState.ATTACK;
        PowerManager.SetLockedAnimation(0);
        State.Attack = GetAttackState(PowerManager.GetCurrentPower());
        AttackControlls();
    }

    eAttackState GetAttackState(ePowerType Power)
    {
        switch(Power)
        {
            case ePowerType.NONE:
                return eAttackState.SWALLOWING;
            case ePowerType.BEAM:
                return eAttackState.BEAM;
            case ePowerType.FIRE:
                return eAttackState.FIRE;
            case ePowerType.ELECTRIC:
                return eAttackState.PRE_ELECTRIC;
            case ePowerType.SPIT:
                return eAttackState.SPITTING;
            default:
                return eAttackState.SWALLOWING;
        }
    }

    void ChangeStateToFat()
    {
        //Set the Animator controller to the Fat State Controller
       // PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Fat;

        State.Current = eControllState.FAT;
        State.Fat = eFatState.IDLE;
        PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Fat;
        FatControls();

    }

    void ChangeStateToHit()
    {
        State.Current = eControllState.HIT;
        PlayerAnimator.runtimeAnimatorController = PlayerAnimatorControllers.Hit;
        HitControlls();
    }

    public void Deflate()
    {
        Deflating = true;
    }


    public void SetDeadly(bool IsDeadly)
    {
        Deadly = IsDeadly;
    }

    public bool IsDeadly()
    {
        return Deadly;
    }

    public void TakeDamage(int Damage, eDamageType Type, GameObject Enemy)
    {
        if(!Invulnerable)
        {
            HP = HP - Damage;
            if (HP <= 0) GameOver();
            else
            {
                Invulnerable = true;
                DamageTaken = Type;
                Movement.SetHitRollDirection((gameObject.transform.position - Enemy.transform.position).normalized);
                ChangeToState(eControllState.HIT);
                PowerManager.CurrentPower = ePowerType.NONE;
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }

    //Debug---------------------------------------------------------------------------------------------------
    string GetSubStateName()
    {
        switch (State.Current)
        {
            case eControllState.BASE:
                return State.Base.ToString();

            case eControllState.CROUCHING:
                return State.Crouching.ToString();

            case eControllState.FLOATING:
                return State.Floating.ToString();

            case eControllState.ATTACK:
                return State.Attack.ToString();

            case eControllState.HIT:
                return State.Hit.ToString();

            case eControllState.FAT:
                return State.Fat.ToString();

            default:
                return "Invalid State";
        }
    }

    private void OnGUI()
    {
        GUILayout.TextArea("State: " + State.Current.ToString());
        GUILayout.TextArea("Sub-State: " + GetSubStateName());
        GUILayout.TextArea("Power: " + PowerManager.GetCurrentPower().ToString());
        GUILayout.TextArea("EnemyPower: " + PowerManager.GetSwallowedEnemyPower().ToString());
        GUILayout.TextArea("HP: " + HP.ToString());
        GUILayout.TextArea("Deadly: " + Deadly.ToString());
    }
}

/*
 ------------------------------------Old Update---------------------------------------------------------------------------

        if(Movement.IsGrounded())
        {
            Movement.MovementState = eBaseState.IDLE;
        }
        else if (Body.velocity.y > 0)
        {
            Movement.MovementState = eBaseState.JUMPING;
        }
        else
        {
            Movement.MovementState = eBaseState.FALLING;
        }
       
        if (Input.GetAxis(Controller.HorizontalAxis) != 0)
        {
            Movement.Walk(Input.GetAxis(Controller.HorizontalAxis));
        }

        if (Input.GetButtonDown(Controller.A))
        {
            Movement.Jump();
        }

        if (Body.velocity.y <= 0)
        {
            Body.velocity += Vector2.up * Physics2D.gravity.y * (Movement.FallGMultiplier - 1) * Time.deltaTime;
        }
        else if (!Input.GetButton(Controller.A))
        {
            Body.velocity += Vector2.up * Physics2D.gravity.y * (Movement.LowJumpGMultiplier - 1) * Time.deltaTime;
        }

            SetAnimatorParameters();

    -------------------------------Old Update End----------------------------------------------------------------------------


        void SetAnimatorParameters()
    {
        PlayerAnimator.SetInteger("MovementState", Movement.MovementState.GetHashCode());

        PlayerAnimator.SetFloat("Speed", Mathf.Abs(Movement.Speed * Input.GetAxis(Controller.HorizontalAxis)));

        if(Movement.MovementState == eBaseState.IDLE && PlayerAnimator.GetFloat("BlinkTimer") < 5.0f)
        {
            PlayerAnimator.SetFloat("BlinkTimer", PlayerAnimator.GetFloat("BlinkTimer") + Time.deltaTime);
        }
        else
        {
            PlayerAnimator.SetFloat("BlinkTimer", 0.0f);
        }
    }


    -------------------------------------------------------------------------------------------------------------------------------
    public class AnimationManager
{
    public void Animate(ControllStateIndex State, Animator PlayerAnimator)
    {
        switch (State.Current)
        {
            case eControllState.BASE:
                Animate(State.Base, PlayerAnimator);
                break;
            case eControllState.CROUCHING:
                Animate(State.Crouching, PlayerAnimator);
                break;
            case eControllState.FLOATING:
                Animate(State.Floating, PlayerAnimator);
                break;
            case eControllState.FAT:
                Animate(State.Fat, PlayerAnimator);
                break;
            case eControllState.HIT:
                Animate(State.Hit, PlayerAnimator);
                break;
        }
    }

    void Animate(eBaseState Base, Animator PlayerAnimator)
    {
        switch (Base)
        {
            case eBaseState.IDLE:
                PlayerAnimator.Play("Kirby_Idle");
                break;
            case eBaseState.WALKING:
                PlayerAnimator.Play("Kirby_Walk");
                break;
            case eBaseState.JUMPING:
                PlayerAnimator.Play("Kirby_Jump");
                break;
            case eBaseState.FALLING:
                PlayerAnimator.Play("Kirby_Fall");
                break;
        }
    }

    void Animate(eCrouchingState Crouching, Animator PlayerAnimator)
    {

    }

    void Animate(eFloatingState Floating, Animator PlayerAnimator)
    {

    }

    void Animate(eFatState Fat, Animator PlayerAnimator)
    {

    }

    void Animate(eHitState Hit, Animator PlayerAnimator)
    {

    }

}
*/

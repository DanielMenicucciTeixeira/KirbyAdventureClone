using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBaseState
{
    IDLE,
    WALKING,
    JUMPING,
    FALLING,
    FALLING_TACKLE,
    RUNNING,
    NUMBER_OF_STATES
}

public enum eFloatingState
{
    FLOATING,
    FLYING,
    DEFLATING,
    NUMBER_OF_STATES
}

public enum eCrouchingState
{
    IDLE,
    SLIDE_TACKLE,
    NUMBER_OF_STATES
}

public enum eAttackState
{
    SWALLOWING,
    SPITTING, 
    BEAM,
    PRE_ELECTRIC,
    FIRE,
    DEFLATING,
    ELECTRIC,
    NUMBER_OF_STATES
}

public enum eTransitionState
{
    INFLATE,
    DEFLATE,
    CROUCH,
    SWALLOW,
    SPIT,
    NUMBER_OF_STATES
}

public enum eFatState
{
    IDLE,
    WALKING,
    JUMPING,
    FALLING,
    SPITTING,
    SWALLOWING,
    NUMBER_OF_STATES
}

public enum eControllState
{
    BASE,
    CROUCHING,
    FLOATING,
    ATTACK,
    FAT,
    HIT,
    NUMBER_OF_STATES
}

public enum eHitState
{
    HIT,
    NUMBER_OF_STATES
}

public struct ControllStateIndex
{
    //The following are all the possible main states of Kirby, each has it's own subsates (movement related)
    public eBaseState Base;
    public eCrouchingState Crouching;
    public eFloatingState Floating;
    public eAttackState Attack;
    public eFatState Fat;
    public eHitState Hit;

    //Enum to controll in wich of the previous main states the player is
    public eControllState Current;
}


/*public class PlayerStateManager : MonoBehaviour
{
    ControllStateIndex State;

    private void Awake()
    {
        State.Base = eBaseState.IDLE;
        State.Crouching = eCrouchingState.IDLE;
        State.Floating = eFloatingState.FLOATING;
        State.Swallowing = eSwallowingState.SWALLOWING;
        State.Fat = eFatState.IDLE;
        State.Hit = eHitState.HIT;

        State.Current = eControllState.BASE;
    }
}*/

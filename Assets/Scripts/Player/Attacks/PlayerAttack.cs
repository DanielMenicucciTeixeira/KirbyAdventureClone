using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePowerType
{
    NONE,
    SPIT,
    BEAM,
    ELECTRIC,
    FIRE,
    DEFLATING,
    NUMBER_OF_POWERS
}

[RequireComponent(typeof(SwallowParticleSystem))]
public class PlayerAttack : MonoBehaviour
{
    public ePowerType CurrentPower = ePowerType.NONE;
    public ePowerType SwallowedEnemyPower = ePowerType.NONE;
    public string SwallowedEnemyName = "NONE";
    bool LockedAnimation = false;
    public BeamAttack BeamBlade;
    Vector2 BeamBladeRelativePosition;
    SwallowParticleSystem SwallowSystem;
    EnemyCharacter PulledEnemy = null;
    PlayerMovement Movement;
    PlayerController Controller;

    //Projectile References
    public BaseProjectile FireProjectile;
    public BaseProjectile GhustProjectile;
    public BaseProjectile StarProjectile;

    public Vector2 VortexBoxSize;
    public Vector2 VortexBoxPosition;
    Vector3 VortexBoxPosition3;
    public float SwallowSpeed;
    public float SwallowRadius;

    public Transform MouthPoint;
    Vector2 MouthPointRelativePosition;

    private void Awake()
    {
        SwallowSystem = GetComponent<SwallowParticleSystem>();
        Movement = GetComponent<PlayerMovement>();
        Controller = GetComponent<PlayerController>();
        VortexBoxPosition3 = VortexBoxPosition;
        MouthPointRelativePosition = MouthPoint.position - gameObject.transform.position;
        BeamBladeRelativePosition = BeamBlade.transform.position - gameObject.transform.position;
        BeamBlade.GetComponent<SpriteRenderer>().forceRenderingOff = true;
    }

    // Update is called once per frame
    void Update()
    {
        SetMouthPointPosition();
    }

    void SetMouthPointPosition()
    {
        Vector2 NewPosition;
        NewPosition.x = MouthPointRelativePosition.x * Movement.GetFaceDirection();
        NewPosition.y = MouthPointRelativePosition.y;
        MouthPoint.position = (Vector2)gameObject.transform.position + NewPosition;

        NewPosition.x =BeamBladeRelativePosition.x * Movement.GetFaceDirection();
        NewPosition.y = BeamBladeRelativePosition.y;
        BeamBlade.transform.position = (Vector2)gameObject.transform.position + NewPosition;
        if (Movement.GetFaceDirection() > 0) BeamBlade.SetFlip(false);
        else BeamBlade.SetFlip(true);
    }
    public void SetPower(ePowerType Power)
    {
        CurrentPower = Power;
    }

    public void AquirePower()
    {
        CurrentPower = SwallowedEnemyPower;
    }

    public ePowerType GetCurrentPower()
    {
        return CurrentPower;
    }

    public ePowerType GetSwallowedEnemyPower()
    {
        return SwallowedEnemyPower;
    }

    public bool IsInLockedAnimation()
    {
        return LockedAnimation;
    }

    public void SetLockedAnimation(int Lock)
    {
        if (Lock == 0) LockedAnimation = false;
        else LockedAnimation = true;
    }

    public void Attack()
    {
        switch (CurrentPower)
        {
            case ePowerType.NONE:
                Swallow();
                break;
            case ePowerType.SPIT:
                StartSpit();
                break;
            case ePowerType.BEAM:
                BeamAttack();
                break;
            case ePowerType.ELECTRIC:
                StartElectricAttack();
                break;
            case ePowerType.FIRE:
                FireAttack();
                break;
            default:
                break;
        }
    }

    public void StopAttack()
    {
        switch (CurrentPower)
        {
            case ePowerType.BEAM:
                StopBeamAttack();
                break;
            case ePowerType.ELECTRIC:
                StopElectricAttack();
                break;
        }
    }

    void Swallow()
    {
        bool FoundAnEnemy = false;

        SwallowSystem.PlayEffect();
        foreach(Collider2D ColliderObject in Physics2D.OverlapBoxAll(MouthPoint.position,VortexBoxSize,0))
        {
            if (!PulledEnemy && ColliderObject.GetComponent<EnemyCharacter>())
            {
                ColliderObject.GetComponent<EnemyMovement>().MovementType = eEnemyMovementType.SWALLOWED;
                ColliderObject.GetComponent<Collider2D>().isTrigger = true;
                ColliderObject.GetComponent<Animator>().enabled = false;
                ColliderObject.GetComponent<Rigidbody2D>().velocity = (Movement.transform.position - ColliderObject.transform.position).normalized * SwallowSpeed;
                PulledEnemy = ColliderObject.GetComponent<EnemyCharacter>();
                FoundAnEnemy = true;
            }
        }

        if (!FoundAnEnemy) PulledEnemy = null;

        if(PulledEnemy && Vector2.Distance(PulledEnemy.transform.position, MouthPoint.position) <= SwallowRadius)
        {
            GetComponent<PlayerController>().ChangeToState(eControllState.FAT);
            SwallowedEnemyPower = PulledEnemy.Power;
            CurrentPower = ePowerType.SPIT;
            SwallowedEnemyName = PulledEnemy.name;
            Destroy(PulledEnemy.gameObject);
        }

        /*foreach(Collider2D ColliderObject in Physics2D.OverlapCircleAll(GetDirectionalPosition(MouthPoint.position), SwallowRadius))
        {
            if (ColliderObject.GetComponent<EnemyCharacter>())
            {
                GetComponent<PlayerController>().ChangeToState(eControllState.FAT);
                SwallowedEnemyPower = ColliderObject.GetComponent<EnemyCharacter>().Power;
                Destroy(ColliderObject.gameObject);
            }
        }*/
    }

    void StartSpit()
    {
        Controller.State.Attack = eAttackState.SPITTING;
        LockedAnimation = true;
    }

    void MidSpit()
    {
        BaseProjectile Star = SpawnProjectile(StarProjectile, MouthPoint.position, MouthPoint.rotation);
        Star.GetComponent<Rigidbody2D>().velocity = Vector2.right * Star.Speed * Movement.GetFaceDirection();
    }

    void EndSpit()
    {
        CurrentPower = ePowerType.NONE;
        Controller.State.Attack = eAttackState.SWALLOWING;
        Controller.ChangeToState(eControllState.BASE);
        LockedAnimation = false;
    }

    void BeamAttack()
    {
        BeamBlade.SetActive(true);
    }

    void StopBeamAttack()
    {
        BeamBlade.SetActive(false);
        BeamBlade.gameObject.transform.rotation = Quaternion.AngleAxis(BeamBlade.StartAngle, Vector3.forward);
    }

    void StartElectricAttack()
    {
        if (Controller.State.Attack == eAttackState.PRE_ELECTRIC || Controller.State.Attack == eAttackState.ELECTRIC) return;
        Controller.State.Attack = eAttackState.PRE_ELECTRIC;
    }

    public void ContinueElectricAttack()
    {
        Controller.SetDeadly(true);
        Controller.State.Attack = eAttackState.ELECTRIC;
    }

    void StopElectricAttack()
    {
        Controller.SetDeadly(false);
    }

    void FireAttack()
    {

    }

    public void GhustAttack()
    {
        BaseProjectile Ghust = SpawnProjectile(GhustProjectile, MouthPoint.position, MouthPoint.rotation);
        Ghust.GetComponent<Rigidbody2D>().velocity = Vector2.right * Ghust.Speed * Movement.GetFaceDirection();
    }

    BaseProjectile SpawnProjectile(BaseProjectile Projectile, Vector3 Position, Quaternion Rotation)
    {
        BaseProjectile NewProjectile = Instantiate(Projectile, MouthPoint.position, Rotation);
        Physics2D.IgnoreCollision(NewProjectile.GetComponent<Collider2D>(), Movement.GetComponent<Collider2D>());
        if (Movement.GetFaceDirection() < 0) NewProjectile.GetComponent<SpriteRenderer>().flipX = true;
        return NewProjectile;
    }

    private void OnDrawGizmos()
    {
        Vector3 NewVortexBoxPosition = VortexBoxPosition;
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position + NewVortexBoxPosition, VortexBoxSize);
    }
}

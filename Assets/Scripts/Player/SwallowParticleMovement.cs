using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class SwallowParticleMovement : MonoBehaviour
{
    Vector3 MouthPoint;
    float ParticleSpeed;
    float ParticleKillRange;

    private void Update()
    {
        transform.position += (MouthPoint - transform.position).normalized * ParticleSpeed * Time.deltaTime;

        if((MouthPoint - transform.position).magnitude < ParticleKillRange)
        {
            Destroy(gameObject);
        }
    }

    public void SetParameters(float Speed, float KillRange, Vector3 Mouth)
    {
        MouthPoint = Mouth;
        ParticleSpeed = Speed;
        ParticleKillRange = KillRange;
    }
}

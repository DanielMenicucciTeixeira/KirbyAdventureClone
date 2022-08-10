using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class SwallowParticleSystem : MonoBehaviour
{
    PlayerMovement Movement;
    public GameObject Particle;

    public Transform MouthPoint;
    Transform SpawnPoint;

    public Vector2 HeightRange;
    public Vector2 LengthRange;
    public float ParticleSpeed;
    public float ParticleKillRange;
    public float ParticlesPerSecond;
    float ParticleSpawnInterval;// Set to 0 for 1 particle per frame
    float ParticleSpawnTimer = 0;

    private void Awake()
    {
        SpawnPoint = Particle.transform;
        Movement = GetComponent<PlayerMovement>();
        ParticleSpawnInterval = 1.0f / ParticlesPerSecond;
    }

    public void PlayEffect()
    {
        if(ParticleSpawnTimer <= 0)
        {
            SpawnPoint.position = MouthPoint.position + new Vector3(Random.Range(LengthRange.x, LengthRange.y) * Movement.GetFaceDirection(), Random.Range(HeightRange.x, HeightRange.y), 0);
            ParticleSpawnTimer = ParticleSpawnInterval;

            GameObject NewParticle = Instantiate(Particle, SpawnPoint.position, SpawnPoint.rotation);
            Vector3 MouthPosition = new Vector3(gameObject.transform.position.x + (Movement.GetFaceDirection() * (MouthPoint.position.x - gameObject.transform.position.x)), MouthPoint.position.y, MouthPoint.position.z);
            NewParticle.GetComponent<SwallowParticleMovement>().SetParameters(ParticleSpeed, ParticleKillRange, MouthPosition);
        }
        else
        {
            ParticleSpawnTimer -= Time.deltaTime;
        }
    }
}

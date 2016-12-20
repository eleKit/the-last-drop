using UnityEngine;
using System.Collections;

public class Particle_Spawner : MonoBehaviour
{
    float SPAWN_INTERVAL = 0.1f; // How much time until the next particle spawns
    float lastSpawnTime = float.MinValue; //The last spawn time
    public GameObject Particle;
    public int PARTICLE_LIFETIME = 3; //How much time will each particle live
    public Vector3 particleForce; //Is there a initial force particles should have?
    public Dynam_Particle.STATES particlesState = Dynam_Particle.STATES.WATER; // The state of the particles spawned
    public Transform particlesParent; // Where will the spawned particles will be parented (To avoid covering the whole inspector with them)

    void Start() { }

    void Update()
    {
        if (lastSpawnTime + SPAWN_INTERVAL < Time.time)
        { // Is it time already for spawning a new particle?
            GameObject newLiquidParticle = POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject(Particle.name); //Spawn a particle
            Dynam_Particle particleScript = newLiquidParticle.GetComponent<Dynam_Particle>(); // Get the particle script
            particleScript.SetLifeTime(PARTICLE_LIFETIME); //Set each particle lifetime
            particleScript.SetState(particlesState); //Set the particle State
            particleScript.get_rb().AddForce(particleForce); //Add our custom force
            newLiquidParticle.transform.position = transform.position;// Relocate to the spawner position
            newLiquidParticle.transform.parent = particlesParent;// Add the particle to the parent container			
            lastSpawnTime = Time.time; // Register the last spawnTime			
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private ParticleSystem _particleSys = null;
    private Rigidbody _rigdBody = null;
    void Start()
    {
        _particleSys = GetComponent<ParticleSystem>();
        _rigdBody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_particleSys != null && !_particleSys.IsAlive(true))
        {
            Destroy(gameObject);          
        }
    }


    void OnParticleCollision(GameObject other)
    {
        _rigdBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
        _particleSys.Stop();
    }
}

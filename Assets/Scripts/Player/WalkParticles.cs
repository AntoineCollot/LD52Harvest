using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkParticles : MonoBehaviour
{
    public GroundCaster ground;
    ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        ground.onGroundedStateChanged.AddListener(OnGroundStateChanged);
        particles = GetComponent<ParticleSystem>();
    }

    private void OnGroundStateChanged(bool isGrounded)
    {
        if (isGrounded)
            particles.Play();
        else
            particles.Stop();
    }
}

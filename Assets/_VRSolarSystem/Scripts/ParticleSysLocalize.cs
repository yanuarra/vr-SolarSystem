using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleSysLocalize : MonoBehaviour
{
    ParticleSystem particle;
    private void Start()
    {
        if (particle == null)
            particle = GetComponent<ParticleSystem>();
        var main = particle.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
    }
}

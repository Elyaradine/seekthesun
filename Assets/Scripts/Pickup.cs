using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pickup : MonoBehaviour
{
    public ParticleSystem ClickParticles;
    private SunSpawner _spawner;

    public void SetSpawner(SunSpawner spawner)
    {
        _spawner = spawner;
    }

    public void OnClick()
    {
        Execute();
        Destroy(gameObject);
    }

    void Execute()
    {
        if (ClickParticles != null)
        {
            Instantiate(ClickParticles, transform.position, Quaternion.identity);
        }

        _spawner.PlantObj.GrowNewStem();
    }
}
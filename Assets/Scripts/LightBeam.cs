using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    public Plant PlantParent;
    public int EnergyContributionPerStem = 2;

    private Collider[] _results;
    private int _energyAdded;
    public bool BeamHit => _energyAdded > 0;

    void Awake()
    {
        _results = new Collider[50];
        _energyAdded = 0;
    }

    void Update()
    {
        var hits = Physics.OverlapCapsuleNonAlloc(transform.position + transform.up * 10.0f,
            transform.position - transform.up * 10.0f, 0.5f, _results);

        
        int energyAdded = 0;
        for (int i = 0; i < hits; i++)
        {
            Stem stem = _results[i].GetComponent<Stem>();
            if (stem != null)
            {
                stem.IsLit = true;
                energyAdded++;
            }
        }

        PlantParent.AddMaxEnergy((energyAdded - _energyAdded) * EnergyContributionPerStem);
        _energyAdded = energyAdded;
    }
}
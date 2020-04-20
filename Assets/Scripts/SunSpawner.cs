using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSpawner : MonoBehaviour
{
    public GameObject SunPickupPrefab;
    public float MinSpawnTime = 2.5f;
    public float MaxSpawnTime = 5.0f;
    public float SpawnHeight = 3.0f;
    public Plant PlantObj;

    private float _ticker;

    void Start()
    {
        _ticker = Random.Range(MinSpawnTime, MaxSpawnTime);
    }

    void Update()
    {
        _ticker -= Time.deltaTime;

        if (_ticker <= 0)
        {
            _ticker = Random.Range(MinSpawnTime, MaxSpawnTime);
            Vector3 pos = Random.onUnitSphere;
            pos.y = SpawnHeight;
            GameObject go = Instantiate(SunPickupPrefab, pos, Quaternion.identity);
            go.GetComponent<Pickup>().SetSpawner(this);
        }
    }
}
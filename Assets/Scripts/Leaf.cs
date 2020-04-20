using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    public Stem ParentStem;
    public float LeafGrowthSpeed = 0.25f;
    public float MaxScale = 3.5f;

    private float _ticker = 0;
    private bool _skipUpdate = false;

    public void UpdateState()
    {
//        if (_skipUpdate)
//        {
//            return;
//        }

        _ticker += Time.deltaTime;

        float scale = _ticker * LeafGrowthSpeed;
        transform.localScale = scale * Vector3.one;

        if (scale >= MaxScale)
        {
            transform.localScale = MaxScale * Vector3.one;
//            _skipUpdate = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static float ElasticOut(float t)
    {
        t = Mathf.Clamp01(t);
        if (t >= 1) return 1;

        return (Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) / 0.3f) + 1);
    }
}
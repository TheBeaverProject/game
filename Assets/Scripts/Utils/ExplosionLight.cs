using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

public class ExplosionLight : MonoBehaviour
{
    private Light light;
    
    void Start()
    {
        if (TryGetComponent<Light>(out light))
        {
            StartCoroutine(Utils.SmoothTransition(f => light.intensity = Mathf.Lerp(1000, 0, f), 0.8f));
        }
    }
}

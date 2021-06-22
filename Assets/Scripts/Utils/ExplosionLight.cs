using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

public class ExplosionLight : MonoBehaviour
{
    private Light _light;
    
    void Start()
    {
        if (TryGetComponent<Light>(out _light))
        {
            StartCoroutine(Utils.SmoothTransition(f => _light.intensity = Mathf.Lerp(1000, 0, f), 0.8f));
        }
    }
}

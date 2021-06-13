using System;
using System.Collections;
using UnityEngine;

namespace Scripts
{
    public class Utils
    {
        public static IEnumerator SmoothTransition(Action<float> transition, float time, Action callback = null)
        {
            float i = 0.0f;
            float rate = 1.0f / time;

            while (i < 1f)
            {
                i += Time.deltaTime * rate;
                transition(i);
                
                yield return null;
            }

            if (callback != null)
                callback();
        }
    }
}
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Scripts
{
    public static class ExtensionMethods
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
    
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
        
        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }
       
            obj.layer = newLayer;
       
            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        public delegate void SpriteCallback(Sprite sprite);
        public static IEnumerator GetSpriteFromUrl(string url, SpriteCallback callback)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            Sprite sprite = null;
            
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.LogWarning(www.error);
            }
            else
            {
                var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                
                sprite = Sprite.Create(
                    texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(0, 0));
            }

            callback(sprite);
        }
        
        
        
        public static async void GetSpriteFromUrlNoCoroutine(string url, SpriteCallback callback)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            Sprite sprite = null;

            await www.SendWebRequest();
                
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogWarning(www.error);
            }
            else
            {
                var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;

                sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0, 0));
            }

            callback(sprite);
        }
    }
}
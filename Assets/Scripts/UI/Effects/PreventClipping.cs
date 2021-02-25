using UnityEngine;

namespace UI.Effects
{
    public class PreventClipping : MonoBehaviour {
        public Shader curShader;
        private Material curMaterial;
        
        Material material
        {
            get
            {
                if(curMaterial == null)
                {
                    curMaterial = new Material(curShader);
                    curMaterial.hideFlags = HideFlags.HideAndDontSave; 
                }
                return curMaterial;
            }
        }
        
        void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if(curShader != null)
            {
                Graphics.Blit(sourceTexture, destTexture, material);
            }
            else
            {
                Graphics.Blit(sourceTexture, destTexture); 
            }
        }

        void OnDisable ()
        {
            if(curMaterial)
            {
                DestroyImmediate(curMaterial); 
            }
        }

    }
}
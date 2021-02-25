namespace UI.Effects
{
    using UnityEngine;
    using System.Collections;
    
    public class ChromaticAberration : MonoBehaviour {
   
        #region Variables
        public Shader curShader;
        public float ChromaticAbberation = 1.0f;
        private Material curMaterial;
        #endregion
   
        #region Properties
        
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
        
        #endregion

        void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if(curShader != null)
            {
                material.SetFloat("_AberrationOffset", ChromaticAbberation);
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
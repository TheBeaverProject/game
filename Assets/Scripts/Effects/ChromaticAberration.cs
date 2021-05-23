namespace UI.Effects
{
    using UnityEngine;
    using System.Collections;
    
    public class ChromaticAberration : MonoBehaviour {
   
        #region Variables

        public bool Enable = false;
        public Shader curShader;
        public Vector2 ChromaticAbberation = new Vector2(0, 0);
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
            if(curShader != null || Enable)
            {
                material.SetFloat("_AberrationOffsetX", ChromaticAbberation.x);
                material.SetFloat("_AberrationOffsetY", ChromaticAbberation.y);
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
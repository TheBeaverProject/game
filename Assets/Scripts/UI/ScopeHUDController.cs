using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScopeHUDController : MonoBehaviour
    {
        public enum scopeType
        {
            LR,
            Acog,
            RedDot,
        }
        
        public GameObject LRImg;
        public GameObject AcogImg;
        public GameObject RedDotImg;

        private void Start()
        {
            LRImg.SetActive(false);
            AcogImg.SetActive(false);
            RedDotImg.SetActive(false);
        }

        public void Toggle(scopeType scope)
        {
            switch (scope)
            {
                case scopeType.LR:
                    LRImg.SetActive(!LRImg.activeInHierarchy);
                    break;
                case scopeType.Acog:
                    AcogImg.SetActive(!AcogImg.activeInHierarchy);
                    break;
                case scopeType.RedDot:
                    RedDotImg.SetActive(!RedDotImg.activeInHierarchy);
                    break;
                    
            }
        }
    }
}
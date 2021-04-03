using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScopeHUDController : MonoBehaviour
    {
        public GameObject LRImg;

        private void Start()
        {
            LRImg.SetActive(false);
        }

        public void Toggle()
        {
            LRImg.SetActive(!LRImg.activeInHierarchy);
        }
    }
}
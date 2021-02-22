using System;
using UnityEngine;

namespace UI.MainMenu.Tabs.HomeTab
{
    public class NewsHandler : MonoBehaviour
    {
        public GameObject newsContainer;
        public GameObject newsPrefab;

        private void OnEnable()
        {
            Firebase.DatabaseHandler.GetAllNews(list =>
            {
                list.Reverse();
                
                int count = 0;
                foreach (var news in list)
                {
                    var container = Instantiate(newsPrefab);
                    var handler = container.GetComponent<NewsContainerHandler>();
                    handler.SetContent(news.Title, news.Content);
                    
                    UnityEditor.GameObjectUtility.SetParentAndAlign(container, newsContainer);
                    container.transform.Translate(0, -10 + -220 * count, 0);
                    count++;
                }
                
                RectTransform contTransform = newsContainer.transform as RectTransform;
                if (contTransform != null)
                {
                    contTransform.sizeDelta = new Vector2(contTransform.sizeDelta.x, 230 * count);
                }
            });
        }
    }
}
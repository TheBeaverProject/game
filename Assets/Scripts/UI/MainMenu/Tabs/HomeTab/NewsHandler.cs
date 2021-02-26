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
 
                    container.transform.position = newsContainer.transform.position;
                    container.transform.Translate(10, -10 + -220 * count, 0);

                    container.GetComponent<NewsContainerHandler>().SetContent(news.Title, news.Content);
                    
                    container.transform.SetParent(newsContainer.transform);
                    
                    count++;
                }
            });
        }
    }
}
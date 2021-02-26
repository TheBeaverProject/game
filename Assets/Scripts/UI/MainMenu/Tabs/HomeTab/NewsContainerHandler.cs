using TMPro;
using UnityEngine;

namespace UI.MainMenu.Tabs.HomeTab
{
    public class NewsContainerHandler : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI contentText;

        public void SetContent(string title, string content)
        {
            titleText.text = title;
            contentText.text = content;
        }
    }
}
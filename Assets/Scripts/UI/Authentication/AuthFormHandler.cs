using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Authentication
{
    public class AuthFormHandler : MonoBehaviour
    {
        public TMP_InputField emailInput;
        public TMP_InputField passwordInput;
        public TextMeshProUGUI errorText;
        public Button submitButton;
        
        public void OnSubmit()
        {
            var email = emailInput.text;
            var password = passwordInput.text;
            
            Firebase.AuthHandler.SignIn(email, password, user =>
            {
                if (user != null)
                {
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    errorText.text = "Authentication failed: invalid email or password.";
                }
            });
        }
    }
}

using System;
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
        public Toggle saveCredentialsToggle;
        public Button submitButton;

        public void Start()
        {
            Firebase.AuthHandler.CheckAuthentication(state =>
            {
                if (state)
                    NextStep();
            });
        }

        public void OnSubmit()
        {
            var email = emailInput.text;
            var password = passwordInput.text;
            
            Firebase.AuthHandler.SignIn(email, password, saveCredentialsToggle.isOn, user =>
            {
                if (user != null)
                    NextStep();
                else
                    errorText.text = "Authentication failed: invalid email or password.";
            });
        }

        public void OnRegister()
        {
            Application.OpenURL("https://thebeaverproject.tk/register"); // TODO: PRODUCTION URL
        }

        private void NextStep()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

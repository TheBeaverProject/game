using System;
using System.Collections.Generic;
using Firebase.Data;
using FullSerializer;
using Proyecto26;
using UnityEngine;

namespace Firebase
{
    public class AuthHandler : MonoBehaviour
    {
        private static readonly fsSerializer Serializer = new fsSerializer();
        
        private const string APIKey = "AIzaSyDVEizX_DZkdCWYht3c7i83z6WbMBgewdU";

        /// <summary>
        /// Saves the provided authentication data in the playerPrefs for future use
        /// </summary>
        /// <param name="localId">userId provided by firebase</param>
        /// <param name="idToken">temporary token provided by firebase</param>
        /// <param name="refreshToken">refresh token provided by firebase</param>
        /// <param name="expiresIn">expiration time of the provided token</param>
        private static void SaveLoggedUserData(string localId, string idToken, string refreshToken, long expiresIn)
        {
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserId, localId);
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserToken, idToken);
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserExpiration, DateTimeOffset.Now.AddSeconds(expiresIn).ToString());
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserRefreshToken, refreshToken);
            
            PlayerPrefs.Save();
        }
        
        public delegate void SignInCallback(User user);
        
        /// <summary>
        /// Signs up the user on firebase with the provided email and password.
        /// Returns an object representing the user if the signin is successful, null otherwise, 
        /// </summary>
        /// <param name="email">Email registered in Firebase</param>
        /// <param name="password">Password associated with the email</param>
        /// <param name="callback">Callback containing the retrieved data</param>
        /// <returns>Object containing the user info</returns>
        public static void SignIn(string email, string password, SignInCallback callback)
        {
            var payload = $"{{\"email\":\"{email}\"," +
                          $"\"password\":\"{password}\"," +
                          $"\"returnSecureToken\":true}}";

            RestClient.Post($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={APIKey}", payload)
                .Then(authRes =>
                {
                    var authResponseJson = authRes.Text;

                    var authResponse = FirebaseAuthResponse.FromJson(authResponseJson);
                    
                    Debug.Log($"Firebase.AuthHandler: User logged in: {authResponse.LocalId}");

                    SaveLoggedUserData(authResponse.LocalId, authResponse.IdToken, authResponse.RefreshToken,
                        authResponse.ExpiresIn);

                    Debug.Log($"Token expiration date: {PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserExpiration)}");

                    DatabaseHandler.GetUserById(authResponse.LocalId, user =>
                    {
                        callback(user);
                    });
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.AuthHandler: Exception when trying to authenticate the user: {err}");
                    callback(null);
                });
        }
        
        
        public delegate void GetNewIdTokenCallback(FirebaseIdTokenResponse response);
        
        /// <summary>
        /// Gets a new Id token after expiration from the refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token provided on login by firebase</param>
        /// <param name="callback">Callback containing the retrieved data</param>
        public static void GetNewIdToken(string refreshToken, GetNewIdTokenCallback callback)
        {
            var payload = $"{{\"grant_type\":\"refresh_token\"," +
                          $"\"refresh_token\":\"{refreshToken}\"}}";

            RestClient.Post($"https://securetoken.googleapis.com/v1/token?key={APIKey}", payload)
                .Then(res =>
                {
                    var jsonString = res.Text;
                    
                    var firebaseIdTokenResponse = FirebaseIdTokenResponse.FromJson(jsonString);
                    
                    SaveLoggedUserData(firebaseIdTokenResponse.UserId, firebaseIdTokenResponse.IdToken, firebaseIdTokenResponse.RefreshToken,
                        firebaseIdTokenResponse.ExpiresIn);

                    callback(firebaseIdTokenResponse);
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.AuthHandler: Exception when trying to authenticate the user: {err}");
                    callback(null);
                });
        }
        
        public delegate void CheckAuthenticationCallback(bool status);
        
        /// <summary>
        /// Method checking if the stored user data is present and still valid
        /// </summary>
        /// <param name="callback">Callback containing the authentication status</param>
        /// <returns></returns>
        public static void CheckAuthentication(CheckAuthenticationCallback callback)
        {
            if (PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserId) is string loggedUserId
                && PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserToken) is string idToken
                && PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserExpiration) is string idTokenExpirationStr
                && PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserRefreshToken) is string loggedRefreshToken) // The necessary keys for authentication are still present. we can therefore proceed
            {
                var idTokenExpiration = DateTimeOffset.Parse(idTokenExpirationStr);

                if (idTokenExpiration.CompareTo(DateTimeOffset.Now) <= 0) // New id token needs to be generated, but auth credentials are still valid
                {
                    GetNewIdToken(loggedRefreshToken, response => { callback(response != null); });
                }
                else
                {
                    callback(true);
                }
            }
            else
            {
                callback(false);
            }
        }
        
    }
}
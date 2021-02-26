using System;
using Firebase.Data;
using Proyecto26;
using UnityEngine;

namespace Firebase
{
    public class AuthHandler : MonoBehaviour
    { 
        private const string APIKey = "AIzaSyDVEizX_DZkdCWYht3c7i83z6WbMBgewdU";

        public static User loggedinUser = null;

        private void OnApplicationQuit()
        {
            if (PlayerPrefs.GetInt(PlayerPrefKeys.SaveCredentials) != 1) // Logout at application quit if the user have chosen not to save credentials 
            {
                LogOut();
            }
        }

        /// <summary>
        /// Saves the provided authentication data in the playerPrefs for future use
        /// </summary>
        /// <param name="localId">userId provided by firebase</param>
        /// <param name="idToken">temporary token provided by firebase</param>
        /// <param name="refreshToken">refresh token provided by firebase</param>
        /// <param name="expiresIn">expiration time of the provided token</param>
        /// <param name="saveCredentials">Save credentials to the disk ?</param>
        private static void SaveLoggedUserData(string localId, string idToken, string refreshToken, long expiresIn, bool saveCredentials)
        {
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserId, localId);
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserToken, idToken);
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserExpiration, DateTimeOffset.Now.AddSeconds(expiresIn).ToString());
            PlayerPrefs.SetString(PlayerPrefKeys.LoggedUserRefreshToken, refreshToken);
            PlayerPrefs.SetInt(PlayerPrefKeys.SaveCredentials, saveCredentials ? 1 : 0);

            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Deletes the saved credentials, having the effect of logging out
        /// </summary>
        public static void LogOut()
        {
            Debug.Log("Successfully logged out");
            
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LoggedUserId);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LoggedUserToken);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LoggedUserExpiration);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LoggedUserRefreshToken);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.SaveCredentials);
            PlayerPrefs.Save();
        }
        
        public delegate void SignInCallback(User user);
        
        /// <summary>
        /// Signs up the user on firebase with the provided email and password.
        /// Returns an object representing the user if the signin is successful, null otherwise, 
        /// </summary>
        /// <param name="email">Email registered in Firebase</param>
        /// <param name="password">Password associated with the email</param>
        /// <param name="saveCredentials">Save credentials to disk ?</param>
        /// <param name="callback">Callback containing the retrieved data</param>
        /// <returns>Object containing the user info</returns>
        public static void SignIn(string email, string password, bool saveCredentials, SignInCallback callback)
        {
            var payload = $"{{\"email\":\"{email}\"," +
                          $"\"password\":\"{password}\"," +
                          $"\"returnSecureToken\":true}}";
            
            RestClient.Post($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={APIKey}", payload)
                .Then(authRes =>
                {
                    var authResponseJson = authRes.Text;

                    var authResponse = FirebaseAuthResponse.FromJson(authResponseJson);

                    SaveLoggedUserData(authResponse.LocalId, authResponse.IdToken, authResponse.RefreshToken,
                        authResponse.ExpiresIn, saveCredentials);
                    
                    Debug.Log($"Firebase.AuthHandler: User logged in: {authResponse.LocalId}");

                    DatabaseHandler.GetUserById(authResponse.LocalId, user =>
                    {
                        loggedinUser = user;
                        callback(user);
                    });
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.AuthHandler: Exception when trying to authenticate the user: {err}");
                    loggedinUser = null;
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
                        firebaseIdTokenResponse.ExpiresIn, PlayerPrefs.GetInt(PlayerPrefKeys.SaveCredentials) == 1);
                    
                    Debug.Log($"Firebase.AuthHandler: Token refreshed");

                    callback(firebaseIdTokenResponse);
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.AuthHandler: Exception when trying to authenticate the user: {err}");
                    loggedinUser = null;
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
            if (PlayerPrefs.HasKey(PlayerPrefKeys.LoggedUserId)
                && PlayerPrefs.HasKey(PlayerPrefKeys.LoggedUserToken)
                && PlayerPrefs.HasKey(PlayerPrefKeys.LoggedUserExpiration)
                && PlayerPrefs.HasKey(PlayerPrefKeys.LoggedUserRefreshToken)) // The necessary keys for authentication are still present. we can therefore proceed
            {
                var idTokenExpirationStr = PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserExpiration);
                
                var idTokenExpiration = DateTimeOffset.Parse(idTokenExpirationStr);

                if (idTokenExpiration.CompareTo(DateTimeOffset.Now) <= 0) // New id token needs to be generated, but auth credentials are still valid
                {
                    var loggedRefreshToken = PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserRefreshToken);
                    
                    GetNewIdToken(loggedRefreshToken, response =>
                    {
                        if (response != null && loggedinUser == null)
                        {
                            DatabaseHandler.GetUserById(response.UserId, user =>
                            {
                                loggedinUser = user;
                                
                                callback(true);
                            });
                        }
                        
                        callback(response != null);
                    });
                }
                else
                {
                    Debug.Log($"Firebase.AuthHandler: User still authenticated");
                    
                    if (loggedinUser == null)
                    {
                        DatabaseHandler.GetUserById(PlayerPrefs.GetString(PlayerPrefKeys.LoggedUserId), user =>
                        {
                            loggedinUser = user;
                            callback(true);
                        });
                    }
                    else
                        callback(true);
                }
            }
            else
            {
                Debug.Log($"Firebase.AuthHandler: Not authenticated anymore");
                loggedinUser = null;
                callback(false);
            }
        }
    }
}
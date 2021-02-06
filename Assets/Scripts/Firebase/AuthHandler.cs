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
                    
                    // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                    // to serialize more complex types (a Dictionary, in this case)
                    var authData = fsJsonParser.Parse(authResponseJson);
                    object authDeserialized = null;
                    Serializer.TryDeserialize(authData, typeof(Dictionary<string, string>), ref authDeserialized);

                    if (!(authDeserialized is Dictionary<string, string> authResponse)) return;
                    
                    Debug.Log($"Firebase.AuthHandler: User logged in: {authResponse["localId"]}");

                    DatabaseHandler.GetUserById(authResponse["localId"], user =>
                    {
                        callback(user);
                    });
                })
                .Catch(err =>
                {
                    Debug.LogErrorFormat($"Firebase.AuthHandler: Exception when trying to authenticate the user: {err}");
                });
        }
    }
}
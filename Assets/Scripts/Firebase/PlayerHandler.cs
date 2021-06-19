using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExitGames.Client.Photon;
using Firebase.Data;
using Photon.Pun;
using Photon.Realtime;
using Proyecto26;
using UnityEngine;

namespace Firebase
{
    public class PlayerHandler : DatabaseHandler
    {
        public static void RefreshLocalPlayerInfo(SuccessCallback callback)
        {
            GetUserById(AuthHandler.loggedinUser._ID, user =>
            {
                AuthHandler.loggedinUser = user;
                
                Hashtable playerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
                if (playerCustomProperties.ContainsKey("elo"))
                    playerCustomProperties["elo"] = AuthHandler.loggedinUser.Elo;
                else
                    playerCustomProperties.Add("elo", AuthHandler.loggedinUser.Elo);
                
                if (playerCustomProperties.ContainsKey("firebaseId"))
                    playerCustomProperties["firebaseId"] = AuthHandler.loggedinUser._ID;
                else
                    playerCustomProperties.Add("firebaseId", AuthHandler.loggedinUser._ID);

                if (playerCustomProperties.ContainsKey("iconUrl"))
                    playerCustomProperties["iconUrl"] = AuthHandler.loggedinUser.IconUrl;
                else
                    playerCustomProperties.Add("iconUrl", AuthHandler.loggedinUser.IconUrl);
                
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
                    
                Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["elo"]);
                    
                Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["firebaseId"]);
                    
                Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["iconUrl"]);

                callback(true);
            });
        }

        public static void UpdateLocalPlayerElo(int newElo, SuccessCallback callback)
        {
            AuthHandler.GetIdToken(token =>
            {
                var document = new FirebaseUserDocument();
                document.Fields.Elo.IntegerValue = newElo;

                var documentStr = Serialize<FirebaseUserDocument>.ToJson(document);
                
                Debug.Log(documentStr);

                RestClient.Request(new RequestHelper
                {
                    Uri = $"{DatabaseUrl}/projects/{ProjectId}/databases/{DatabaseId}/documents/users/{AuthHandler.loggedinUser._ID}" +
                          $"?updateMask.fieldPaths=elo",
                    Method = "PATCH",
                    BodyString = documentStr,
                    Headers = new Dictionary<string, string>
                    {
                        {"Authorization", "Bearer " + token}
                    }
                }).Then(res =>
                {
                    if (res.StatusCode == 200)
                    {
                        PlayerHandler.RefreshLocalPlayerInfo((success =>  callback(success)));
                    }
                    else
                    {
                        Debug.Log(res.StatusCode);
                        Debug.Log(res.Error);
                        Debug.Log(res.Text);
                        callback(false);
                    }
                }).Catch(err =>
                {
                    Debug.LogErrorFormat(
                        $"Firebase.EloHandler: Exception when trying to update elo: {err}");
                    callback(false);
                });
            });
        }

        public static int GetEloUpdate(Player localPlayer, bool won, List<Player> players)
        {
            int averageElo = players.Sum(player => (int) player.CustomProperties["elo"]) / players.Count;
            int localPlayerElo = (int) localPlayer.CustomProperties["elo"];
            int difference = localPlayerElo - averageElo;
            int clamped = won ? Mathf.Clamp(-1 * difference, -100, 100) : Mathf.Clamp(-1 * difference, 100, -100);

            return won ? Mathf.Clamp((int) (clamped * 1.5), 10, 100) : Mathf.Clamp((int) (clamped * 1.2), -100, -10);
        }
    }
}
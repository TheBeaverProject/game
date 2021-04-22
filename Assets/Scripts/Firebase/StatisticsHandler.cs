using System;
using System.Collections.Generic;
using Firebase.Data;
using Photon.Pun.UtilityScripts;
using Proyecto26;
using Scripts;
using UnityEngine;

namespace Firebase
{
    public class StatisticsHandler : DatabaseHandler
    {
        public delegate void PostNewMatchCallback(bool success);

        public static void PostNewMatch(string gameMode, string winner, GameData gameData, PostNewMatchCallback callback)
        {
            FirebaseMatchDocument document = new FirebaseMatchDocument
            {
                Name = "",
                CreateTime = DateTimeOffset.Now,
                UpdateTime = DateTimeOffset.Now,
                Fields = new FirebaseMatchDocumentFields
                {
                    Type = new Data.String{ StringValue = gameMode },
                    Winner = new Data.String{ StringValue = winner },
                    EndDate = new EndDate{ TimestampValue = DateTimeOffset.Now },
                    Players = new Players
                    {
                        ArrayValue = new ArrayValue
                        {
                            Values = new List<Value>()
                        }
                    }
                }
            };

            foreach (var kvp in gameData.Dictionary)
            {
                var team = kvp.Key.GetPhotonTeam();
                
                var value = new Value
                {
                    MapValue = new MapValue
                    {
                        Fields = new MapValueFields
                        {
                            Uid = new Data.String{ StringValue = "" },
                            Name = new Data.String{ StringValue = kvp.Key.NickName },
                            Assists = new Number{ IntegerValue = kvp.Value.assists },
                            Deaths = new Number{ IntegerValue = kvp.Value.deaths },
                            Kills = new Number{ IntegerValue = kvp.Value.kills },
                            Points = new Number{ IntegerValue = kvp.Value.points },
                            Team = team != null ? new Number {IntegerValue = team.Code} : new Number {IntegerValue = 0},
                        }
                    }
                };

                document.Fields.Players.ArrayValue.Values.Add(value);
            }
            
            string documentStr = Serialize<FirebaseMatchDocument>.ToJson(document);
            
            Debug.Log(documentStr);

            AuthHandler.GetIdToken(token =>
            {
                var options = new RequestHelper();
                options.BodyString = documentStr;
                options.Headers.Add("Authorization", "Bearer " + token);
                options.Uri = $"{DatabaseUrl}/projects/{ProjectId}/databases/{DatabaseId}/documents/matches";
                
                RestClient.Post(options)
                    .Then(res =>
                    {
                        if (res.StatusCode == 200)
                        {
                            callback(true);
                        }
                        else
                        {
                            Debug.Log(res.StatusCode);
                            Debug.Log(res.Error);
                            Debug.Log(res.Text);
                        }
                    });
            });
        }
    }
}
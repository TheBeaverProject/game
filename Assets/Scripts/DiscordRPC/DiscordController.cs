using System;
using UnityEngine;
using Discord;

namespace DiscordRPC
{
    public class DiscordController : MonoBehaviour
    {
        public Discord.Discord discord;
        public ActivityManager activityManager;
        private long CLIENT_ID = 838326563275407381;
        
        private void Start()
        {
            InitDiscord();
        }

        private void Update()
        {
            discord.RunCallbacks();
        }

        private void OnDestroy()
        {
            discord.Dispose();
        }

        public void ClearActivity()
        {
            activityManager.ClearActivity((res) =>
            {
                if (res == Discord.Result.Ok)
                {
                    Debug.Log("DiscordRPC.DiscordController: Cleared Activity!");
                }
                else
                {
                    Debug.LogError($"DiscordRPC.DiscordController: {res}");
                }
            });
        }

        public void InitDiscord()
        {
            discord = new Discord.Discord(CLIENT_ID, (System.UInt64) Discord.CreateFlags.Default);
            activityManager = discord.GetActivityManager();
        }

        public void UpdateActivity(string Details, string State, string LargeImage = "logo512",
            string LargeText = "TheBeaverProject", long endTimestamp = -1)
        {
            ActivityAssets asset = new ActivityAssets
            {
                LargeImage = LargeImage,
                LargeText = LargeText
            };

            ActivityTimestamps timestamps = new ActivityTimestamps();

            if (endTimestamp != -1)
            {
                timestamps = new ActivityTimestamps
                {
                    End = endTimestamp
                };
            }

            if (LargeImage != "logo512")
            {
                asset = new ActivityAssets
                {
                    LargeImage = LargeImage,
                    LargeText = LargeText,
                    SmallImage = "logo512",
                    SmallText = "TheBeaverProject"
                };
            }

            var activity = new Discord.Activity
            {
                State = State,
                Details = Details,
                Assets = asset,
                Timestamps = timestamps,
            };

            if (discord == null)
            {
                InitDiscord();
            }
            
            activityManager.UpdateActivity(activity, (res) => {});
        }
    }
}
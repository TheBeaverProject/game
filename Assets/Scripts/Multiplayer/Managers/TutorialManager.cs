using System;
using System.Collections;
using Guns;
using Multiplayer;
using Photon.Pun;
using PlayerManagement;
using UI;
using UI.BuyMenu;
using UI.HUD;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Scripts.Gamemodes
{
    public class TutorialManager : GameManager
    {
        public Launcher launcher;

        public GameObject AIPlayer;

        public Transform[] AISpawnPoints;

        public VideoPlayer videoPlayer;

        private void Start()
        {
            PhotonNetwork.OfflineMode = true;
            
            launcher.Connect();

            videoPlayer = FindObjectOfType<VideoPlayer>();

            StartCoroutine(PlayTrailer());
        }
        
        public bool ReadyToSpawn = false;
        public PlayerManager playerManager;

        private bool skippedVideo = false;
        private void Update()
        {
            if (ReadyToSpawn && PlayerManager.LocalPlayerInstance == null)
            {
                playerManager = RespawnPlayer();

                if (PlayerPrefs.GetInt(PlayerPrefKeys.HasDoneTutorial) == 0 && !Step1Finished)
                {
                    StartCoroutine(TutorialStep1());
                }
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                skippedVideo = true;
                StopVideo();
            }

            if (playerManager != null)
            {
                if (playerManager.Health <= 0)
                {
                    playerManager = RespawnPlayer();
                }
            }

            if (Step1Finished && !Step2Started)
            {
                StartCoroutine(TutorialStep2());
            } else if (Step2Finished && !Step3Started)
            {
                StartCoroutine(TutorialStep3());
            } else if (Step3Finished && !EndStarted)
            {
                if (!Bot1.activeInHierarchy && !Bot2.activeInHierarchy)
                {
                    PlayerPrefs.SetInt(PlayerPrefKeys.HasDoneTutorial, 1);
                    StartCoroutine(TutorialEnd());
                }
            }
        }
        
        PlayerManager RespawnPlayer()
        {
            if (playerManager != null)
            {
                PhotonNetwork.Destroy(playerManager.gameObject);
                PlayerManager.LocalPlayerInstance = null;
                playerManager = null;
            }

            return InstantiateLocalPlayer();
        }

        IEnumerator PlayTrailer()
        {
            videoPlayer.Play();
            
            yield return new WaitForSeconds(101);

            if (!skippedVideo)
            {
                StopVideo();
            }
        }

        void StopVideo()
        {
            ReadyToSpawn = true;
            videoPlayer.Stop();
            videoPlayer.GetComponentInParent<Canvas>().gameObject.SetActive(false);
        }

        private bool Step1Finished = false;
        IEnumerator TutorialStep1()
        {
            playerManager.HUD.Init(HUDType.Teams);

            playerManager.HUD.DisplayAnnouncement("Welcome to The Beaver Project!");
                
            playerManager.HUD.DisplayAnnouncement("You can move using the W A S D keys.");
                
            playerManager.HUD.DisplayAnnouncement("Press B to buy a weapon.");

            yield return new WaitForSeconds(12);

            if (playerManager.playerWeapon == null)
            {
                Debug.Log("Weapon not bought. Restarting Step 1");
                StartCoroutine(TutorialStep1());
            }
            else
            {
                Debug.Log("Weapon bought. Finished Step 1");
                Step1Finished = true;
            }
        }
        
        private bool Step2Started = false;
        private bool Step2Finished = false;
        IEnumerator TutorialStep2()
        {
            Step2Started = true;
            
            yield return new WaitForSeconds(1);
            
            playerManager.HUD.DisplayAnnouncement("Press the Left Click to Fire your weapon.");
            
            playerManager.HUD.DisplayAnnouncement("If your Weapon allows it, Right Clicking aims it.");

            playerManager.HUD.DisplayAnnouncement("Press G to launch a Grenade. Aim well or it will kill you.");
            
            yield return new WaitForSeconds(12);

            if (playerManager.playerCameraHolder.GetComponent<GrenadeThrow>().cooldown < 20)
            {
                // Grenade launched
                Step2Finished = true;
            }
            else
            {
                StartCoroutine(TutorialStep3());
            }
        }
        
        private bool Step3Started = false;
        private bool Step3Finished = false;
        private GameObject Bot1;
        private GameObject Bot2;
        
        IEnumerator TutorialStep3()
        {
            Step3Started = true;
            
            yield return new WaitForSeconds(1);
            
            playerManager.HUD.DisplayAnnouncement("You have two ways of winning this tutorial.");
            
            playerManager.HUD.DisplayAnnouncement("You can either kill the two Bots that will spawn using your arsenal.");
            
            playerManager.HUD.DisplayAnnouncement("Or you can capture the two zones.");

            yield return new WaitForSeconds(10);
            
            playerManager.HUD.DisplayAnnouncement("Spawning the Bots.");

            Bot1 = PhotonNetwork.Instantiate(AIPlayer.name, 
                AISpawnPoints[0].position, AISpawnPoints[0].rotation);
            
            Bot2 = PhotonNetwork.Instantiate(AIPlayer.name, 
                AISpawnPoints[1].position, AISpawnPoints[1].rotation);

            Step3Finished = true;
        }

        private bool EndStarted = false;
        IEnumerator TutorialEnd()
        {
            EndStarted = true;
            playerManager.HUD.DisplayAnnouncement("Congratulations! You've finished the tutorial.");
            
            playerManager.HUD.DisplayAnnouncement("You will be teleported to the Main Menu soon ...");

            yield return new WaitForSeconds(4);
            
            LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
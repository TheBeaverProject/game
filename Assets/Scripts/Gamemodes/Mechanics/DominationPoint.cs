using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Gamemodes.Mechanics
{
    public class DominationPoint : MonoBehaviour
    {
        [Header("Settings")]
        public int radius = 10;

        public int selectedLayer = 14;
        
        private int layerMask;

        [FormerlySerializedAs("particleSystem")]
        public ParticleSystem _particleSystem;
        public Color defaultColor = new Color(0, 0, 0, 0);
        public Color teamColor1 = new Color(0, 235, 248);
        public Color teamColor2 = new Color(229, 0, 0);
        
        private PhotonTeam _dominatingPhotonTeam;

        /// <summary>
        /// Team which currently has the most players in the zone
        /// </summary>
        public PhotonTeam GetDominatingPhotonTeam => _dominatingPhotonTeam;

        // Start is called before the first frame update
        void Start()
        {
            _dominatingPhotonTeam = null;
            var _mainModule = _particleSystem.main;
            _mainModule.startColor = defaultColor;
            layerMask = 1 << selectedLayer;
        }

        // Update is called once per frame
        void Update()
        {
            Collider[] surroundings = Physics.OverlapSphere(transform.position, radius, layerMask);

            PhotonTeam newDominatingTeam = _dominatingPhotonTeam;
            
            if (surroundings.Length != 0)
            {
                PhotonTeam team1 = surroundings[0].GetComponent<PhotonView>().Controller.GetPhotonTeam();

                PhotonTeam team2 = null;
                for (int i = 1; i < surroundings.Length; i++)
                {
                    var controller = surroundings[i].GetComponent<PhotonView>().Controller;
                    if (controller.GetPhotonTeam() != team1)
                    {
                        team2 = controller.GetPhotonTeam();
                        break;
                    }
                }

                
                if (team2 == null)
                {
                    newDominatingTeam = team1;
                }
                else
                {
                    int t1 = 0;
                    int t2 = 0;
                    foreach (Collider c in surroundings)
                    {
                        if (c.GetComponent<PhotonView>().Controller.GetPhotonTeam() == team1)
                            t1++;
                        else
                            t2++;
                    }

                    if (t1 == t2)
                        newDominatingTeam = null;
                    else
                        newDominatingTeam = t1 > t2 ? team1 : team2;
                }
            }
            else
            {
                newDominatingTeam = null;
            }

            if (newDominatingTeam != _dominatingPhotonTeam)
            {
                _dominatingPhotonTeam = newDominatingTeam;
                var _mainModule = _particleSystem.main;
                var color = _dominatingPhotonTeam == null ? defaultColor : _dominatingPhotonTeam.Code == 1 ? teamColor1 : teamColor2;
                _mainModule.startColor = color;
            }
        }
    }

    public abstract class DominationGamemode : Gamemode
    {
        [Header("Zone Domination Mechanics")]
        
        public Mechanics.DominationPoint ZoneA;
        public Mechanics.DominationPoint ZoneB;

        public int[] ZoneAPoints = {0, 0};
        public int[] ZoneBPoints = {0, 0};

        public byte ZoneACapturedBy = 0;
        public byte ZoneBCapturedBy = 0;

        public int PointsToCaptureZone = 700;
        
        protected void HandleZones()
        {
            ComputeZone(ZoneA, ZoneAPoints, ref ZoneACapturedBy, ResetZoneA);
            ComputeZone(ZoneB, ZoneBPoints, ref ZoneBCapturedBy, ResetZoneB);
        }

        public delegate void ResetZone();
        private void ComputeZone(DominationPoint Zone, int[] ZonePoints, ref byte ZoneCapturedBy, ResetZone resetZone)
        {
            PhotonTeam dTeam;
            // Zone is captured by nobody
            if (ZoneCapturedBy == 0) 
            { 
                // Decrement the points of both team
                ZonePoints[0] = ZonePoints[0] > 0 ? ZonePoints[0] - 1 : 0;
                ZonePoints[1] = ZonePoints[1] > 0 ? ZonePoints[1] - 1 : 0;

                // A team is dominating the zone
                if ((dTeam = Zone.GetDominatingPhotonTeam) != null)
                {
                    // Increment the points of the dominating team by two to mitigate the previous decrement
                    int teamI = dTeam.Code - 1;
                    ZonePoints[teamI] += 2;

                    // the number of points reaches the capture treshold
                    if (ZonePoints[teamI] >= PointsToCaptureZone)
                    {
                        ZonePoints[teamI] = PointsToCaptureZone;
                        
                        // dominating team captures the zone
                        ZoneCapturedBy = dTeam.Code;
                        OnZoneCaptured(Zone, ZoneCapturedBy);
                    }
                }
            }
            // Zone is captured and dominated 
            else if ((dTeam = Zone.GetDominatingPhotonTeam) != null)
            {
                int teamI = ZoneCapturedBy - 1;
                // Zone is dominated by the team that has NOT captured it
                if (dTeam.Code != ZoneCapturedBy)
                {
                    // Decrements the points of the team that has captured the zone as long as they are above 0
                    ZonePoints[teamI] = ZonePoints[teamI] > 0 ? ZonePoints[teamI] - 1 : 0;
                
                    // the points of the team that has captured the zone reach 0
                    if (ZonePoints[teamI] <= 0)
                    {
                        // The zone is reset
                        resetZone();
                    }
                }
                // Zone is dominated by the team that has captured it
                else
                {
                    // Increment the points of the team that has captured it if it is below the capture treshold
                    ZonePoints[teamI] = ZonePoints[teamI] < PointsToCaptureZone ? ZonePoints[teamI] + 1 : ZonePoints[teamI];
                }
            }
            
            // default color of the zone is modified to show that is has been captured
            Zone.defaultColor = ZoneCapturedBy == 0 ? new Color(0, 0, 0, 0) :
                ZoneCapturedBy == 1 ? Zone.teamColor1 : Zone.teamColor2;
        }

        protected virtual void OnZoneCaptured(DominationPoint Zone, byte zoneCapturedBy) {}

        protected bool AreZoneCaptured => ZoneACapturedBy != 0 && ZoneACapturedBy == ZoneBCapturedBy;

        protected void ResetZones()
        {
            ResetZoneA();
            ResetZoneB();
        }

        protected void ResetZoneA()
        {
            ZoneA.defaultColor = new Color(0, 0, 0, 0);
            ZoneAPoints = new []{0, 0};
            ZoneACapturedBy = 0;
        }
        
        protected void ResetZoneB()
        {
            ZoneB.defaultColor = new Color(0, 0, 0, 0);
            ZoneBPoints = new []{0, 0};
            ZoneBCapturedBy = 0;
        }
    }
}

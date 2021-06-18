using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PlayerManagement;
using UnityEngine;

namespace Scripts.Gamemodes.Mechanics
{
    public class DominationPoint : MonoBehaviour
    {
        [Header("Settings")]
        public int radius = 10;

        public int selectedLayer = 14;
        
        private int layerMask;

        new public ParticleSystem particleSystem;
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
            var _mainModule = particleSystem.main;
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
                var _mainModule = particleSystem.main;
                var color = _dominatingPhotonTeam == null ? defaultColor : _dominatingPhotonTeam.Code == 1 ? teamColor1 : teamColor2;
                _mainModule.startColor = color;
            }
        }
    }
}

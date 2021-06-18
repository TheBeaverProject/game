using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class DominationPoint : MonoBehaviour
{
    
    private PhotonTeam _photonTeam;
    
    // Start is called before the first frame update
    void Start()
    {
        _photonTeam = null;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] surroundings = Physics.OverlapSphere(transform.position, 10, 10);
        if (surroundings.Length != 0)
        {
            PhotonTeam team1 = surroundings[0].GetComponent<PhotonView>().Controller.GetPhotonTeam();
            PhotonTeam team2 = null;
            for (int i = 1; i < surroundings.Length; i++)
            {
                if (surroundings[i].GetComponent<PhotonView>().Controller.GetPhotonTeam() != team1)
                {
                    team2 = surroundings[i].GetComponent<PhotonView>().Controller.GetPhotonTeam();
                    break;
                }
            }

            if (team2 == null)
            {
                _photonTeam = team1;
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
                    {
                        t2++;
                    }
                }

                if (t1 == t2)
                {
                    _photonTeam = null;
                }
                else
                {
                    _photonTeam = t1 > t2 ? team1 : team2;
                }
            }
        }
    }
}

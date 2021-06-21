using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncementsDisplay : MonoBehaviour {
    public GameObject AnnoncementPrefab;

    public List<GameObject> AnnoucementsList;

    private float currTime = 0;
    private void Update()
    {
        if (AnnoucementsList.Count > 0)
        {
            var curr = AnnoucementsList[0];

            if (!curr.activeInHierarchy)
                curr.SetActive(true);

            currTime += Time.deltaTime;

            if (currTime > 3f)
            {
                currTime = 0;
                Destroy(curr);
                AnnoucementsList.Remove(curr);
            }
        }
    }

    public void AddAnnouncement(string text)
    {
        var announcement = Instantiate(AnnoncementPrefab, this.transform);
        announcement.SetActive(false);
        announcement.GetComponentInChildren<TextMeshProUGUI>().text = text;
        AnnoucementsList.Add(announcement);
    }
}
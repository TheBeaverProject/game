using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

public class KillFeedDisplay : MonoBehaviour
{
    public GameObject KillFeedElementPrefab;

    private Vector3 lastElPos = Vector3.zero;
    private Vector3 elHeight = new Vector3(0, 30, 0);
    private Color32 highlightColor = new Color32(246, 235, 20, 255);
    private List<GameObject> elList = new List<GameObject>();
    
    /// <summary>
    /// Adds a kill display to the killfeed
    /// </summary>
    /// <param name="name1">name of the killers</param>
    /// <param name="name2">name of the killed</param>
    /// <param name="iconColor">color of the icon</param>
    /// <param name="highlight1">highlight the killers</param>
    /// <param name="highlight2">highlight the killed</param>
    public void AddElement(string name1, string name2, Color iconColor, bool highlight1 = false, bool highlight2 = false)
    {
        foreach (var o in elList)
            o.transform.localPosition -= elHeight;

        var el = Instantiate(KillFeedElementPrefab, this.transform, false);
        elList.Add(el);
        
        var kfEl = el.GetComponent<KillFeedElement>();

        kfEl.icon.color = iconColor;
        kfEl.name1.text = name1;
        kfEl.name2.text = name2;

        kfEl.name1.color = highlight1 ? (Color) highlightColor : Color.white;
        kfEl.name2.color = highlight2 ? (Color) highlightColor : Color.white;
        
        var routine = StartCoroutine(RemoveElement(el));
    }

    private IEnumerator RemoveElement(GameObject el)
    {
        yield return new WaitForSeconds(5f);
        var vec = new Vector3(300, 0, 0);
        
        StartCoroutine(Utils.SmoothTransition(f =>
        {
            var time = Mathf.SmoothStep(0.0f, 0.1f, f);
            el.transform.localPosition = Vector3.Lerp(el.transform.localPosition, el.transform.localPosition + vec, time);
        }, 0.3f, () =>
        {
            elList.Remove(el);
            Destroy(el);
        }));
    }
}

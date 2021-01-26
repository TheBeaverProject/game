using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealhDisplay : MonoBehaviour
{
    private int health = 100;
    public Text healthText;
    
    // Start is called before the first frame update
    void Start()
    {
        healthText.text = $"Health: {health}";
    }

    // Update is called once per frame
    private void Update()
    {
        healthText.text = $"Health: {health}";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            health--;
        }
    }
}

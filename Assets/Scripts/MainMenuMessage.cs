using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using TMPro;
using UnityEngine;

public class MainMenuMessage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start ()
    {
        if (SceneController.GameFinished)
        {
            gameObject.GetComponent<TextMeshProUGUI>().text = "Thank you for playing :)";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

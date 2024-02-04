using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    int FPS = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 200), ((int)(1.0f / Time.smoothDeltaTime)).ToString());
    }


}

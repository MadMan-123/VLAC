using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public Camera PlayerCamera;
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


}

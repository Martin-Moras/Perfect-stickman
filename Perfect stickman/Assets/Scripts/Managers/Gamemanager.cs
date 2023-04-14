using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{

    void Start()
    {
        Transform parent = GameObject.Find("Controll panel").transform;
        CreateUiStickman.CreateStickman(GameObject.Find("Stickman"), parent);

    }
    void Update()
    {
        
    }
}

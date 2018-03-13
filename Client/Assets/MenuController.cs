using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{



    void Awake()
    {
        Debug.Log("Test Awake");
    }

    void OnEnable()
    {
        Debug.Log("Test OnEnable");
    }

    void OnDisable()
    {
        Debug.Log("Test OnDisable");
    }
} 
﻿using UnityEngine;
using System.Collections;

public class ClickTargetting : MonoComponent 
{
    private GameObject clickedPoint;

    void Start () 
    {
        clickedPoint = (GameObject)Instantiate(Resources.Load("ClickPoint"));
    }
    
    void Update () 
    {
        if (Input.GetMouseButton(1))
        {
            Ray start = this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(start, out hit))
            {
                clickedPoint.transform.position = hit.point;
            }
        }
    }
}

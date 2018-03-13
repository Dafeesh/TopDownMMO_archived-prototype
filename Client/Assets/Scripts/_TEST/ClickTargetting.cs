using UnityEngine;
using System.Collections;

public class ClickTargetting : MonoBehaviour 
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
            Ray start = this.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(start, out hit))
            {
                clickedPoint.transform.position = hit.point;
            }
        }
	}
}

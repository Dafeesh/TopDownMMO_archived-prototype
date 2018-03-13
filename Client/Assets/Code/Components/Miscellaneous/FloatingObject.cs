using UnityEngine;
using System.Collections;

public class FloatingObject : MonoBehaviour
{
    private Vector3 floatPos = new Vector3();
    private Quaternion floatRot = new Quaternion();
    private float yStart;
    private float yrStart;

    void Start()
    {
        floatPos = this.transform.position;
        floatRot = this.transform.rotation;
        yStart = this.transform.position.y;
        yrStart = this.transform.rotation.y;

        GameObject.Find("_MainCamera").GetComponent<Camera>().transform.position = new Vector3(-1.0f, 3.0f, 5.0f);
        GameObject.Find("_MainCamera").GetComponent<Camera>().transform.rotation = Quaternion.Euler(41, 160, 0);
    }

    void Update()
    {
        floatRot.y = yrStart + 0.1f * Mathf.Sin(Time.time % 360);
        this.transform.rotation = floatRot;

        floatPos.y = yStart + 0.1f*Mathf.Sin((Time.time % 180) * 2);
        this.transform.position = floatPos;
    }
}

using UnityEngine;
using System.Collections;

public class TEST_ReportCreateAndDestroy : MonoBehaviour
{
    void Start()
    {
        Debug.Log("CREATED [" + this.GetHashCode() + "]: " + System.DateTime.Now.ToString("G"));
    }

    void OnDestroy()
    {
        Debug.Log("DESTROYED [" + this.GetHashCode() + "]: " + System.DateTime.Now.ToString("G"));
    }
}

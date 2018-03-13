using UnityEngine;
using System.Collections.Generic;

public class ConsoleController : MonoComponent
{
    public UnityEngine.UI.Text textBox;

    private int numberOfLines;
    private List<string> posts = new List<string>();
    private string posts_report;

    void Start()
    {
        SetNumberOfLines(5);
    }

    public void SetNumberOfLines(int n)
    {
        numberOfLines = n;
    }

    public void Post(string s)
    {
        posts.Add(s);
        posts_report = "";
        for (int i=posts.Count-1; i>=0 && i>posts.Count-1-numberOfLines; i--)
        {
            posts_report += "[" + System.DateTime.Now.ToString("hh:mm:ss tt") + "]: " + posts[i] + "\n";
        }
        textBox.text = posts_report;
    }
}

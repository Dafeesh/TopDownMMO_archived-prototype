using UnityEngine;

using Extant;

public class MonoComponent : MonoBehaviour , ILogging
{
    private DebugLogger _log;

    public MonoComponent()
    {
        _log = new DebugLogger(this.GetType().Name);
    }

    public DebugLogger Log
    {
        get
        {
            return _log;
        }
    }
}

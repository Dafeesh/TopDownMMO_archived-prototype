using System;

public enum ConnectionState
{
    Null,
    NoConnection,
    Connecting,
    Authorizing,
    Connected
}

public delegate void Delegate_ConnectionStateChange(ConnectionState state);
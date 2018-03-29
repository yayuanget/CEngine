using System;
using UnityEngine;

public interface ILogger
{
    void Log(string msg, string stack, LogType type);
}


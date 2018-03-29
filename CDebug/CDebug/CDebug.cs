using System;
using System.Text;
using UnityEngine;

public static class CDebug
{
    public static bool useLog = true;
    public static string threadStack = string.Empty;
    public static ILogger logger = null;
    private static string LogFormat(string str)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[" + DateTime.Now.ToString("MM-dd hh:mm:ss") + "]:");
        sb.Append(str);
        return StringBuilderCache.GetStringAndRelease(sb);
    }
    public static void Log(string str)
    {
        str = LogFormat(str);
        if (useLog)
        {
            Debug.Log(str);
            return;
        }
        if (logger != null)
        {
            logger.Log(str, string.Empty, LogType.Log);
        }
    }

    public static void Log(object message)
    {
        Log(message.ToString());
    }
    public static void Log(string str, object arg0)
    {
        Log(string.Format(str, arg0));
    }
    public static void Log(string str, object arg0, object arg1)
    {
        Log(string.Format(str, arg0, arg1));
    }
    public static void Log(string str, object arg0, object arg1, object arg2)
    {
        Log(string.Format(str, arg0, arg1, arg2));
    }
    public static void Log(string str, params object[] param)
    {
        Log(string.Format(str, param));
    }
    public static void LogWarning(string str)
    {
        str = LogFormat(str);
        if (useLog)
        {
            Debug.LogWarning(str);
            return;
        }
        if (logger != null)
        {
            string stack = StackTraceUtility.ExtractStackTrace();
            logger.Log(str, stack, LogType.Warning);
        }
    }
    public static void LogWarning(object message)
    {
        LogWarning(message.ToString());
    }
    public static void LogWarning(string str, object arg0)
    {
        LogWarning(string.Format(str, arg0));
    }
    public static void LogWarning(string str, object arg0, object arg1)
    {
        LogWarning(string.Format(str, arg0, arg1));
    }
    public static void LogWarning(string str, object arg0, object arg1, object arg2)
    {
        LogWarning(string.Format(str, arg0, arg1, arg2));
    }
    public static void LogWarning(string str, params object[] param)
    {
        LogWarning(string.Format(str, param));
    }
    public static void LogError(string str)
    {
        str = LogFormat(str);
        if (useLog)
        {
            Debug.LogError(str);
            return;
        }
        if (logger != null)
        {
            string stack = StackTraceUtility.ExtractStackTrace();
            logger.Log(str, stack, LogType.Error);
        }
    }
    public static void LogError(object message)
    {
        LogError(message.ToString());
    }
    public static void LogError(string str, object arg0)
    {
        LogError(string.Format(str, arg0));
    }
    public static void LogError(string str, object arg0, object arg1)
    {
        LogError(string.Format(str, arg0, arg1));
    }
    public static void LogError(string str, object arg0, object arg1, object arg2)
    {
        LogError(string.Format(str, arg0, arg1, arg2));
    }
    public static void LogError(string str, params object[] param)
    {
        LogError(string.Format(str, param));
    }
    public static void LogException(Exception e)
    {
        threadStack = e.StackTrace;
        string logFormat = LogFormat(e.Message);
        if (useLog)
        {
            Debug.LogError(logFormat);
            return;
        }
        if (logger != null)
        {
            logger.Log(logFormat, threadStack, LogType.Exception);
        }
    }
    public static void LogException(string str, Exception e)
    {
        threadStack = e.StackTrace;
        str = LogFormat(str + e.Message);
        if (useLog)
        {
            Debug.LogError(str);
            return;
        }
        if (logger != null)
        {
            logger.Log(str, threadStack, LogType.Exception);
        }
    }
}


namespace SFCore;

internal static class InternalLogger
{
    private static string FormatOriginator(object originator, string message)
    {
        if (originator == null)
        {
            return message;
        }
        if (originator is string)
        {
            return $"{originator} - {message}";
        }
        return $"[{originator.GetType().FullName?.Replace(".", "]:[")}] - {message}";
    }
    internal static void LogFine(string message, object originator = null)
    {
        string formattedMessage = FormatOriginator(originator, message);
        Modding.Logger.LogFine(formattedMessage);
        UnityEngine.Debug.Log(formattedMessage);
    }
    internal static void LogDebug(string message, object originator = null)
    {
        string formattedMessage = FormatOriginator(originator, message);
        Modding.Logger.LogDebug(formattedMessage);
        UnityEngine.Debug.Log(formattedMessage);
    }
    internal static void Log(string message, object originator = null)
    {
        string formattedMessage = FormatOriginator(originator, message);
        Modding.Logger.Log(formattedMessage);
        UnityEngine.Debug.Log(formattedMessage);
    }
    internal static void LogWarn(string message, object originator = null)
    {
        string formattedMessage = FormatOriginator(originator, message);
        Modding.Logger.LogWarn(formattedMessage);
        UnityEngine.Debug.LogWarning(formattedMessage);
    }
    internal static void LogError(string message, object originator = null)
    {
        string formattedMessage = FormatOriginator(originator, message);
        Modding.Logger.LogError(formattedMessage);
        UnityEngine.Debug.LogError(formattedMessage);
    }
}

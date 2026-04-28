using System.IO;
using UnityEngine;

public static class AnalyticsManager
{
    static string path = Application.persistentDataPath + "/analytics.csv";

    public static void Log(string eventName, string details = "")
    {
        string line = Time.time + "," + eventName + "," + details + "\n";
        File.AppendAllText(path, line);
    }
}
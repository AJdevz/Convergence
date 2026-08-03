using System.IO;
using UnityEngine;

public static class AnalyticsManager
{
    private static string folderPath =
        Path.Combine(Application.persistentDataPath, "TelemetryLogs");

    private static string path;

    private static bool headerWritten = false;


    public static void Initialise()
    {
        // Create folder if it doesn't exist
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);


        // Create unique filename using date/time
        string fileName =
            "Telemetry_" +
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") +
            ".csv";


        path = Path.Combine(folderPath, fileName);


        File.WriteAllText(path,
            "Time,FPS,Wave,ActiveEnemies,MergedEnemies,MergeEvents,PlayerHP\n");


        headerWritten = true;


        Debug.Log("Telemetry saved to: " + path);
    }


    public static void LogTelemetry(
        int fps,
        int wave,
        int activeEnemies,
        int mergedEnemies,
        int mergeEvents,
        float playerHP)
    {
        if (!headerWritten)
            Initialise();


        string line =
            $"{Time.time:F1}," +
            $"{fps}," +
            $"{wave}," +
            $"{activeEnemies}," +
            $"{mergedEnemies}," +
            $"{mergeEvents}," +
            $"{playerHP:F0}\n";


        File.AppendAllText(path, line);
    }


    public static string GetFilePath()
    {
        return path;
    }
}
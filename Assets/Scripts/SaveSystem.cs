using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string path => Application.persistentDataPath + "/save.json";

    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static PlayerData Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }

        return new PlayerData();
    }
}
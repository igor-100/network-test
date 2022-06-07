using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class EventsSaveSystem
{
    private static readonly string crashedEventsFolder = Application.persistentDataPath + "/CrashedEvents/";

    public static void Init()
    {
        if (!Directory.Exists(crashedEventsFolder))
        {
            Directory.CreateDirectory(crashedEventsFolder);
        }
    }

    public static void RemoveAll()
    {
        if (Directory.Exists(crashedEventsFolder))
        {
            var di = new DirectoryInfo(crashedEventsFolder);
            var files = di.GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }
        }
    }

    public static bool ContainsAnyFiles()
    {
        return Directory.Exists(crashedEventsFolder) && Directory.GetFiles(crashedEventsFolder).Length > 0;
    }

    public static void Save(string saveString)
    {
        int saveNumber = 1;

        while (File.Exists(crashedEventsFolder + "events_" + saveNumber + ".json"))
        {
            saveNumber++;
        }
        File.WriteAllText(crashedEventsFolder + "events_" + saveNumber + ".json", saveString);
    }

    public static List<string> Load()
    {
        List<string> saveFiles = new List<string>();

        foreach (string file in Directory.EnumerateFiles(crashedEventsFolder))
        {
            saveFiles.Add(File.ReadAllText(file));
        }

        return saveFiles;
    }
}

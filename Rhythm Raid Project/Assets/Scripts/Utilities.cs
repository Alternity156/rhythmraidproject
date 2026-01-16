using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Utilities : MonoBehaviour
{
    public static Utilities I { get; private set; }

    private void Awake()
    {
        I = this;
    }

    public float PixelsPerSeconds(float time, float pixels)
    {
        return pixels / time;
    }

    public string[] GetFilePathsInFolder(string folderPath)
    {
        Debug.Log($"Retrieving file paths from {folderPath}...");

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath, "*.*");
            return files;
        }
        else
        {
            Debug.LogError("Directory not found");
            return new string[0];
        }
    }

    public string[] GetFoldersInLevels()
    {
        string[] directories = Directory.GetDirectories(GameManager.I.levelsPath);
        return directories;
    }

    public string CreateLevelFolder(string levelName)
    {
        Debug.Log($"Creating folder for level \"{levelName}\"");

        string levelPath = Path.Combine(GameManager.I.levelsPath, levelName);
        int count = 1;
        bool folderCreated = false;

        while (!folderCreated)
        {
            if (count != 1)
            {
                levelPath = Path.Combine(GameManager.I.levelsPath, $"{levelName}{count}");
            }

            if (!Directory.Exists(levelPath))
            {
                Directory.CreateDirectory(levelPath);
                folderCreated = true;

                Debug.Log("Created level folder");
            }
            else
            {
                count += 1;
            }
        }

        return levelPath;
    }

    public Level CreateLevel(string songName, string artistName, string audioFilePath, float startingBPM)
    {
        Level level = new Level();

        string folderPath = new string(songName.Where(char.IsLetterOrDigit).ToArray());
        folderPath = CreateLevelFolder(folderPath.ToLower().Replace(" ", ""));

        string audioFileName = Path.GetFileName(audioFilePath);
        string destinationAudioFilePath = Path.Combine(folderPath, audioFileName);

        TempoMarker tempoMarker = new TempoMarker();
        tempoMarker.time = 0;
        tempoMarker.tempo = startingBPM;

        List<TempoMarker> tempoMarkers = new List<TempoMarker>();
        tempoMarkers.Add(tempoMarker);

        File.Copy(audioFilePath, destinationAudioFilePath);

        level.artistName = artistName;
        level.songName = songName;
        level.folderPath = folderPath;
        level.audioFile = audioFileName;
        level.tempoMarkers = tempoMarkers;

        return level;
    }

    public Level SaveLevelData(Level level)
    {
        string jsonString = JsonConvert.SerializeObject(level, Formatting.Indented);
        File.WriteAllText(Path.Combine(level.folderPath, GameManager.I.dataFileName), jsonString);

        return level;
    }

    public Level LoadLevelData(string levelFolderPath)
    {
        string jsonString = File.ReadAllText(Path.Combine(levelFolderPath, GameManager.I.dataFileName));
        Level level = JsonConvert.DeserializeObject<Level>(jsonString);

        return level;
    }

    public class Level
    {
        public string folderPath;
        public string audioFile;
        public string songName;
        public string artistName;
        public List<TempoMarker> tempoMarkers;
    }

    public class TempoMarker
    {
        public float tempo;
        public float time;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

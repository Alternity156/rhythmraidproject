using UnityEngine;
using System.IO;

public class Utilities : MonoBehaviour
{
    public static Utilities I { get; private set; }

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

    private void Awake()
    {
        I = this;
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

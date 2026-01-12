using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject editorMenu;

    static string songsFolder = "Songs";

    void StartState()
    {
        Debug.Log("Setting up start state");
        mainMenu.SetActive(true);
        editor.SetActive(false);
        editorMenu.SetActive(false);
    }

    void FolderSetup()
    {
        string songsPath = Path.Combine(Application.dataPath, songsFolder);

        if (!Directory.Exists(songsPath))
        {
            Directory.CreateDirectory(songsPath);
            Debug.Log("Created Songs folder");
        }
        else
        {
            Debug.Log("Songs folder exists");
        }
    }

    private void Awake()
    {
        I = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartState();
        FolderSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

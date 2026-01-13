using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    public GameState gameState = GameState.MainMenu;

    public Utilities.Level level;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject editorMenu;
    [SerializeField] private GameObject editorMenuCanvas;
    [SerializeField] private GameObject newProjectCanvas;

    public static string songsFolder = "Songs";
    public string songsPath = Path.Combine(Application.dataPath, songsFolder);

    public static string levelsFolder = "Levels";
    public string levelsPath = Path.Combine(Application.dataPath, levelsFolder);

    public string dataFileName = "data.json";

    public enum GameState
    {
        MainMenu,
        Editor
    }

    void StartState()
    {
        Debug.Log("Setting up start state");
        mainMenu.SetActive(true);
        editor.SetActive(false);
        editorMenu.SetActive(false);
        editorMenuCanvas.SetActive(false);
        newProjectCanvas.SetActive(false);
    }

    void FolderSetup()
    {
        if (!Directory.Exists(songsPath))
        {
            Directory.CreateDirectory(songsPath);
            Debug.Log("Created Songs folder");
        }
        else
        {
            Debug.Log("Songs folder exists");
        }

        if (!Directory.Exists(levelsPath))
        {
            Directory.CreateDirectory(levelsPath);
            Debug.Log("Created Levels folder");
        }
        else
        {
            Debug.Log("Levels folder exists");
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

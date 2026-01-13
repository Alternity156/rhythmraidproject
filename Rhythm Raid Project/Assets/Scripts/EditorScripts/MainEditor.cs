using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainEditor : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject editorCanvas;
    [SerializeField] private GameObject editorMenu;
    [SerializeField] private GameObject editorMenuCanvas;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject newProjectCanvas;
    [SerializeField] private GameObject newProjectScroll;
    [SerializeField] private GameObject newProjectFormCanvas;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button newButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_InputField audioFileInput;
    [SerializeField] private TMP_InputField songNameInput;
    [SerializeField] private TMP_InputField artistNameInput;
    [SerializeField] private TextMeshProUGUI infoSongLabel;
    [SerializeField] private TextMeshProUGUI infoArtistLabel;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform contentParent;

    private List<Button> buttons = new List<Button>();

    Button SetupButtonPrefab(string text)
    {
        Button newButton = Instantiate(buttonPrefab, contentParent);
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = text;

        return newButton;
    }

    void GenerateButtonsNew(string[] filePaths)
    {
        foreach (string filePath in filePaths)
        {
            if (filePath.EndsWith(".ogg"))
            {
                Button newButton = SetupButtonPrefab(Path.GetFileName(filePath));
                newButton.onClick.AddListener(() => OnNewButtonClick(filePath));
                buttons.Add(newButton);
            }
        }
    }

    void GenerateButtonsLoad(string[] folderPaths)
    {
        foreach (string folderPath in folderPaths)
        {
            Button newButton = SetupButtonPrefab(folderPath.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar).Last());
            newButton.onClick.AddListener(() => OnLoadButtonClick(folderPath));
            buttons.Add(newButton);
        }
    }

    void OnNewButtonClick(string filePath)
    {
        newProjectScroll.SetActive(false);
        newProjectFormCanvas.SetActive(true);

        audioFileInput.text = filePath;

        Debug.Log($"Opening {filePath}");
    }

    void OnLoadButtonClick(string folderPath)
    {
        newProjectScroll.SetActive(false);
        editorCanvas.SetActive(true);
        ClearButtons();

        GameManager.I.level = Utilities.I.LoadLevelData(folderPath);
        AudioManager.I.LoadAudio(Path.Combine(folderPath, GameManager.I.level.audioFile));

        AudioManager.I.SetVolume(volumeSlider.value);

        infoSongLabel.text = $"Song: {GameManager.I.level.songName}";
        infoArtistLabel.text = $"Artist: {GameManager.I.level.artistName}";
    }

    private void Confirm()
    {
        Utilities.Level level = Utilities.I.CreateLevel(songNameInput.text, artistNameInput.text, audioFileInput.text);
        Utilities.I.SaveLevelData(level);

        newProjectFormCanvas.SetActive(false);

        OnLoadButtonClick(level.folderPath);
    }

    private void ClearButtons()
    {
        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }

        buttons.Clear();
    }

    private void CancelNew()
    {
        ClearButtons();

        newProjectCanvas.SetActive(false);
        editorCanvas.SetActive(true);
    }

    void OpenMenu()
    {
        Debug.Log("Opening editor menu...");

        editorCanvas.SetActive(false);
        editorMenu.SetActive(true);
        editorMenuCanvas.SetActive(true);
    }

    private void Exit()
    {
        editorMenu.SetActive(false);
        editor.SetActive(false);
        mainMenu.SetActive(true);

        GameManager.I.gameState = GameManager.GameState.MainMenu;
    }

    private void Back()
    {
        editorMenu.SetActive(false);
        editor.SetActive(true);
    }

    private void New()
    {
        newProjectCanvas.SetActive(true);
        newProjectScroll.SetActive(true);
        newProjectFormCanvas.SetActive(false);
        editorMenuCanvas.SetActive(false);
        GenerateButtonsNew(Utilities.I.GetFilePathsInFolder(GameManager.I.songsPath));
    }

    private void Load()
    {
        newProjectCanvas.SetActive(true);
        newProjectScroll.SetActive(true);
        newProjectFormCanvas.SetActive(false);
        editorMenuCanvas.SetActive(false);
        GenerateButtonsLoad(Utilities.I.GetFoldersInLevels());
    }

    private void Play()
    {
        AudioManager.I.Play();
    }

    private void Pause()
    {
        AudioManager.I.Pause();
    }

    private void Stop()
    {
       AudioManager.I.Stop();
    }

    private void SetVolume(float volume)
    {
        AudioManager.I.SetVolume(volume);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuButton.onClick.AddListener(OpenMenu);
        exitButton.onClick.AddListener(Exit);
        backButton.onClick.AddListener(Back);
        newButton.onClick.AddListener(New);
        cancelButton.onClick.AddListener(CancelNew);
        confirmButton.onClick.AddListener(Confirm);
        loadButton.onClick.AddListener(Load);
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        stopButton.onClick.AddListener(Stop);

        volumeSlider.onValueChanged.AddListener(delegate { SetVolume(volumeSlider.value); } );
    }

    // Update is called once per frame
    void Update()
    {

    }
}

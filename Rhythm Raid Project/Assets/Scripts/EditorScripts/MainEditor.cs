using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainEditor : MonoBehaviour
{
    public static MainEditor I { get; private set; }

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
    [SerializeField] private TMP_InputField startingBPMInput;
    [SerializeField] private TextMeshProUGUI infoSongLabel;
    [SerializeField] private TextMeshProUGUI infoArtistLabel;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] public Slider positionSlider;

    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform contentParent;

    private List<Button> buttons = new List<Button>();
    private List<TextMeshProUGUI> bpmLabels = new List<TextMeshProUGUI>();
    private List<GameObject> bpmLines = new List<GameObject>();

    [SerializeField] private Canvas waveformCanvas;
    [SerializeField] private Image waveformImage;
    [SerializeField] private TextMeshProUGUI bpmLabelPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Color waveformColor = Color.green;
    [SerializeField] private Color waveformBackgroundColor = Color.black;

    private RectTransform waveformCanvasRectTransform;

    public static int waveformWidth = 16384;
    private int waveformHeight = 100;
    private float waveformCanvasStartPos = waveformWidth / 2;
    private float waveformSaturation = 0.5f;
    [HideInInspector] public float waveformPixelsPerSecond;

    private void Awake()
    {
        I = this;
    }

    public int GetWaveformWidth()
    {
        return waveformWidth;
    }

    public void PlaceBeatLines()
    {
        List<float> beatPixels = new List<float>();

        Utilities.TempoMarker currentBpm = GameManager.I.level.tempoMarkers[0];

        float currentTime = 0f;
        float bpmTime = Utilities.I.GetBeatTime(currentBpm.tempo);
        float songLength = AudioManager.I.GetLength();
        int nextTempoMarkerIndex = 1;
        int beatIncrements = 4;

        while (currentTime < songLength)
        {
            if (GameManager.I.level.tempoMarkers.Count > nextTempoMarkerIndex)
            {
                Utilities.TempoMarker nextBpm = GameManager.I.level.tempoMarkers[nextTempoMarkerIndex];

                if (currentTime >= nextBpm.time)
                {
                    currentBpm = nextBpm;
                    bpmTime = Utilities.I.GetBeatTime(currentBpm.tempo);
                    nextTempoMarkerIndex += 1;
                }
            }

            float beatPixel = waveformPixelsPerSecond * currentTime;
            beatPixels.Add(beatPixel);
            currentTime += bpmTime * beatIncrements;
            Debug.Log($"{currentTime} < {songLength}");
        }

        foreach (float beatPixel in beatPixels)
        {
            GameObject line = Instantiate(linePrefab, waveformCanvas.transform);
            RectTransform lineRectTransform = line.GetComponent<RectTransform>();

            float linePositionX = beatPixel - (waveformWidth / 2);

            lineRectTransform.localPosition = new Vector3(linePositionX, lineRectTransform.localPosition.y, lineRectTransform.localPosition.z);

            bpmLines.Add(line);
        }
    }

    public void SetWaveformCanvasPosition()
    {
        waveformCanvasRectTransform.sizeDelta = new Vector2(waveformWidth, 140);
        waveformCanvasRectTransform.localPosition = new Vector3(waveformCanvasStartPos, waveformCanvasRectTransform.localPosition.y, waveformCanvasRectTransform.localPosition.z);
        waveformImage.rectTransform.localPosition = new Vector3(0, waveformImage.rectTransform.localPosition.y, waveformImage.rectTransform.localPosition.z);
    }

    public void ApplyWaveFormTexture()
    {
        Texture2D texture = Utilities.I.PaintWaveformSpectrum(AudioManager.I.audioSource.clip, waveformSaturation, waveformWidth, waveformHeight, waveformColor, waveformBackgroundColor);
        waveformImage.rectTransform.sizeDelta = new Vector2(waveformWidth, waveformHeight);
        waveformImage.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void ApplyBpmLabels()
    {
        foreach (Utilities.TempoMarker tempoMarker in GameManager.I.level.tempoMarkers)
        {
            float labelPositionX = (tempoMarker.time * waveformPixelsPerSecond) - (waveformWidth / 2);

            TextMeshProUGUI bpmLabel = Instantiate(bpmLabelPrefab, waveformCanvas.transform);
            bpmLabel.text = tempoMarker.tempo.ToString();
            bpmLabel.rectTransform.localPosition = new Vector3(labelPositionX, bpmLabel.rectTransform.localPosition.y, bpmLabel.rectTransform.localPosition.z);

            bpmLabels.Add(bpmLabel);
        }
    }

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
        Utilities.Level level = Utilities.I.CreateLevel(songNameInput.text, artistNameInput.text, audioFileInput.text, float.Parse(startingBPMInput.text));
        Utilities.I.SaveLevelData(level);

        newProjectFormCanvas.SetActive(false);

        OnLoadButtonClick(level.folderPath);
    }

    private void ClearBpmLabels()
    {
        foreach (TextMeshProUGUI bpmLabel in bpmLabels)
        {
            Destroy(bpmLabel.gameObject);
        }

        bpmLabels.Clear();
    }

    private void ClearBpmLines()
    {
        foreach (GameObject bpmLine in bpmLines)
        {
            Destroy(bpmLine.gameObject);
        }

        bpmLines.Clear();
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

    private void SetAudioPosition(float position)
    {
        AudioManager.I.Seek(position);
        HandleWaveformPosition();
    }

    private void HandleWaveformPosition()
    {
        waveformCanvasRectTransform.localPosition = new Vector3(-(waveformPixelsPerSecond * positionSlider.value) + waveformCanvasStartPos,
                                                                waveformCanvasRectTransform.localPosition.y,
                                                                waveformCanvasRectTransform.localPosition.z);
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
        positionSlider.onValueChanged.AddListener(delegate { SetAudioPosition(positionSlider.value); } );

        waveformCanvasRectTransform = waveformCanvas.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(AudioManager.I.audioLoaded && AudioManager.I.IsPlaying())
        {
            positionSlider.value = AudioManager.I.GetPosition();
            HandleWaveformPosition();
        }
    }
}

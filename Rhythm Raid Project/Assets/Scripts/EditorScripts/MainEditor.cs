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

    [SerializeField] private Canvas waveformCanvas;
    [SerializeField] private Image waveformImage;
    [SerializeField] private Color waveformColor = Color.green;
    [SerializeField] private Color waveformBackgroundColor = Color.black;

    private RectTransform waveformCanvasRectTransform;

    public static int waveformWidth = 16384;
    private int waveformHeight = 100;
    private float waveformCanvasStartPos = waveformWidth / 2;
    private float waveformSaturation = 0.5f;
    public float waveformPixelsPerSecond;

    private void Awake()
    {
        I = this;
    }

    public int GetWaveformWidth()
    {
        return waveformWidth;
    }

    public void SetWaveformCanvasPosition()
    {
        waveformCanvasRectTransform.sizeDelta = new Vector2(waveformWidth, 140);
        waveformCanvasRectTransform.localPosition = new Vector3(waveformCanvasStartPos, waveformCanvasRectTransform.localPosition.y, waveformCanvasRectTransform.localPosition.z);
        waveformImage.rectTransform.localPosition = new Vector3(0, waveformImage.rectTransform.localPosition.y, waveformImage.rectTransform.localPosition.z);
    }

    public void ApplyWaveFormTexture()
    {
        Texture2D texture = PaintWaveformSpectrum(AudioManager.I.audioSource.clip, waveformSaturation, waveformWidth, waveformHeight, waveformColor, waveformBackgroundColor);
        waveformImage.rectTransform.sizeDelta = new Vector2(waveformWidth, waveformHeight);
        waveformImage.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Debug.Log(waveformImage.rectTransform.position.x);
        Debug.Log(waveformImage.rectTransform.localPosition.x);
    }

    Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color wfColor, Color backgroundColor)
    {
        audio = AudioManager.I.ConvertAudioClipToMono(audio, "mono");

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples];
        float[] waveform = new float[width];
        audio.GetData(samples, 0);
        int packSize = (audio.samples / width) + 1;
        int s = 0;

        for (int i = 0; i < audio.samples; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, backgroundColor);
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * 0.75f); y++)
            {
                tex.SetPixel(x, (height / 2) + y, wfColor);
                tex.SetPixel(x, (height / 2) - y, wfColor);
            }
        }

        tex.Apply();

        return tex;
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

    private void SetAudioPosition(float position)
    {
        AudioManager.I.Seek(position);
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
        if(AudioManager.I.audioLoaded)
        {
            positionSlider.value = AudioManager.I.GetPosition();
            waveformCanvasRectTransform.localPosition = new Vector3(-(Utilities.I.PixelsPerSeconds(AudioManager.I.GetLength(), waveformWidth) * AudioManager.I.GetPosition()) + waveformCanvasStartPos,
                                                                    waveformCanvasRectTransform.localPosition.y,
                                                                    waveformCanvasRectTransform.localPosition.z);
        }
    }
}

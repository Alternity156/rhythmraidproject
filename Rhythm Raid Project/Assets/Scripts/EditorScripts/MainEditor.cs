using System.Collections.Generic;
using System.IO;
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
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button newButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform contentParent;

    private List<Button> buttons = new List<Button>();

    void GenerateButtons(string[] filePaths)
    {
        foreach (string filePath in filePaths)
        {
            if (filePath.EndsWith(".ogg"))
            {
                Button newButton = Instantiate(buttonPrefab, contentParent);
                string fileName = Path.GetFileName(filePath);
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = fileName;
                newButton.onClick.AddListener(() => OnButtonClick(filePath));
                buttons.Add(newButton);
            }
        }
    }

    void OnButtonClick(string filePath)
    {
        Debug.Log($"Opening {filePath}");
    }

    private void CancelNew()
    {
        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }

        buttons.Clear();

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
    }

    private void Back()
    {
        editorMenu.SetActive(false);
        editor.SetActive(true);
    }

    private void New()
    {

        newProjectCanvas.SetActive(true);
        editorMenuCanvas.SetActive(false);
        GenerateButtons(Utilities.I.GetFilePathsInFolder(GameManager.I.songsPath));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuButton.onClick.AddListener(OpenMenu);
        exitButton.onClick.AddListener(Exit);
        backButton.onClick.AddListener(Back);
        newButton.onClick.AddListener(New);
        cancelButton.onClick.AddListener(CancelNew);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

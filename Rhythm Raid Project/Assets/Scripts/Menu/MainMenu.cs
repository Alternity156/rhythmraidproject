using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button editorButton;
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject editorCanvas;

    void QuitGame()
    {
        Debug.Log("Quitting game...");

        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OpenEditor()
    {
        Debug.Log("Opening editor...");

        mainMenu.SetActive(false);
        editor.SetActive(true);
        editorCanvas.SetActive(true);

        GameManager.I.gameState = GameManager.GameState.Editor;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quitButton.onClick.AddListener(QuitGame);
        editorButton.onClick.AddListener(OpenEditor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

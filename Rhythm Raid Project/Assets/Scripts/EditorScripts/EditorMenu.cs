using UnityEngine;
using UnityEngine.UI;

public class EditorMenu : MonoBehaviour
{
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject editorMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exitButton.onClick.AddListener(Exit);
        backButton.onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

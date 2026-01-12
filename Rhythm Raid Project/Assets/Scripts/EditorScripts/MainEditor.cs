using UnityEngine;
using UnityEngine.UI;

public class MainEditor : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject editorMenu;

    void OpenMenu()
    {
        Debug.Log("Opening editor menu...");

        editor.SetActive(false);
        editorMenu.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuButton.onClick.AddListener(OpenMenu);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

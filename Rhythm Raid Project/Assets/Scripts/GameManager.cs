using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject editorMenu;

    void StartState()
    {
        mainMenu.SetActive(true);
        editor.SetActive(false);
        editorMenu.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

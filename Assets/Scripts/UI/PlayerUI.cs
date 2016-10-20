using UnityEngine;

public class PlayerUI : MonoBehaviour 
{

    [SerializeField]
    GameObject pauseMenu;

    void Start()
    {
        PauseMenu.IsOn = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }
}

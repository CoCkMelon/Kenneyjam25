using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu3D : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject optionsUI;
    public string playSceneName = "SleightSpeed";
    
    [Header("Animation Settings")]
    public float hoverScaleMultiplier = 1.2f;
    public float clickScaleMultiplier = 0.8f;
    public float hoverTweenDuration = 0.2f;
    public float clickTweenDuration = 0.1f;
    public float actionDelay = 0.1f;
    
    [Header("Menu Buttons")]
    public MenuButton3D playButton;
    public MenuButton3D optionsButton;
    public MenuButton3D quitButton;

    OptionsMenuController optionsMenuController;

    private void Start()
    {
        optionsMenuController = optionsUI.GetComponent<OptionsMenuController>();
        // Initialize menu buttons
        if (playButton != null)
        {
            playButton.Initialize(MenuAction.Play, this);
        }
        
        if (optionsButton != null)
        {
            optionsButton.Initialize(MenuAction.Options, this);
        }
        
        if (quitButton != null)
        {
            quitButton.Initialize(MenuAction.Quit, this);
        }
        
        // Ensure options UI is closed at start
        if (optionsUI != null)
        {
        optionsMenuController.ApplySettings = Show;
            optionsUI.SetActive(false);
        }

    }
    
    public void ExecuteMenuAction(MenuAction action)
    {
        switch (action)
        {
            case MenuAction.Play:
                StartCoroutine(LoadPlayScene());
                break;
            case MenuAction.Options:
                OpenOptionsUI();
                break;
            case MenuAction.Quit:
                QuitGame();
                break;
        }
    }
    
    private IEnumerator LoadPlayScene()
    {
        yield return new WaitForSeconds(actionDelay);
        
        if (!string.IsNullOrEmpty(playSceneName))
        {
            SceneManager.LoadScene(playSceneName);
        }
        else
        {
            Debug.LogWarning("Play scene name is not set!");
        }
    }
    
    private void OpenOptionsUI()
    {
        if (optionsUI != null)
        {
            playButton.gameObject.GetComponent<SphereCollider>().enabled=false;
            optionsButton.gameObject.GetComponent<SphereCollider>().enabled=false;
            quitButton.gameObject.GetComponent<SphereCollider>().enabled=false;
            gameObject.SetActive(false);
            optionsUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Options UI GameObject is not assigned!");
        }
    }

    public void Show()
    {
        playButton.gameObject.GetComponent<SphereCollider>().enabled=true;
        optionsButton.gameObject.GetComponent<SphereCollider>().enabled=true;
        quitButton.gameObject.GetComponent<SphereCollider>().enabled=true;
        gameObject.SetActive(true);
        optionsMenuController.gameObject.SetActive(false);
    }
    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

public enum MenuAction
{
    Play,
    Options,
    Quit
}

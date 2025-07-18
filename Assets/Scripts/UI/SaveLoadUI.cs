using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SaveLoadUI : MonoBehaviour
{
    [Header("UI References")]
    public Button saveButton;
    public Button loadButton;
    public Button newGameButton;
    public Button deleteSaveButton;
    public Text saveStatusText;
    public Text saveInfoText;
    
    [Header("Save Info Panel")]
    public GameObject saveInfoPanel;
    public Text sceneNameText;
    public Text playtimeText;
    public Text lastSaveTimeText;
    public Text levelText;
    
    void Start()
    {
        SetupButtons();
        UpdateSaveInfo();
    }
    
    void SetupButtons()
    {
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveGame);
            
        if (loadButton != null)
            loadButton.onClick.AddListener(LoadGame);
            
        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);
            
        if (deleteSaveButton != null)
            deleteSaveButton.onClick.AddListener(DeleteSave);
    }
    
    void UpdateSaveInfo()
    {
        if (GameLoader.Instance.HasSaveData())
        {
            SaveData saveData = SaveSystem.LoadGame();
            
            if (saveInfoPanel != null)
                saveInfoPanel.SetActive(true);
                
            if (sceneNameText != null)
                sceneNameText.text = $"Scene: {saveData.currentScene}";
                
            if (playtimeText != null)
            {
                int hours = Mathf.FloorToInt(saveData.playtime / 3600f);
                int minutes = Mathf.FloorToInt((saveData.playtime % 3600f) / 60f);
                playtimeText.text = $"Playtime: {hours:00}:{minutes:00}";
            }
            
            if (lastSaveTimeText != null)
                lastSaveTimeText.text = $"Last Save: {saveData.lastSaveTime:MM/dd/yyyy HH:mm}";
                
            if (levelText != null)
                levelText.text = $"Health: {saveData.playerHealth:F0}/{saveData.playerMaxHealth:F0}";
            
            if (loadButton != null)
                loadButton.interactable = true;
                
            if (deleteSaveButton != null)
                deleteSaveButton.interactable = true;
        }
        else
        {
            if (saveInfoPanel != null)
                saveInfoPanel.SetActive(false);
                
            if (loadButton != null)
                loadButton.interactable = false;
                
            if (deleteSaveButton != null)
                deleteSaveButton.interactable = false;
        }
    }
    
    public void SaveGame()
    {
        try
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuickSave();
                ShowStatus("Game Saved Successfully!", Color.green);
            }
            else
            {
                GameLoader.Instance.QuickSave();
                ShowStatus("Game Saved Successfully!", Color.green);
            }
            
            UpdateSaveInfo();
        }
        catch (Exception e)
        {
            ShowStatus($"Save Failed: {e.Message}", Color.red);
            Debug.LogError($"Save error: {e}");
        }
    }
    
    public void LoadGame()
    {
        try
        {
            if (GameLoader.Instance.HasSaveData())
            {
                GameLoader.Instance.LoadSavedGame();
                ShowStatus("Game Loaded Successfully!", Color.green);
            }
            else
            {
                ShowStatus("No save data found!", Color.red);
            }
        }
        catch (Exception e)
        {
            ShowStatus($"Load Failed: {e.Message}", Color.red);
            Debug.LogError($"Load error: {e}");
        }
    }
    
    public void StartNewGame()
    {
        try
        {
            GameLoader.Instance.StartNewGame();
            ShowStatus("New Game Started!", Color.green);
        }
        catch (Exception e)
        {
            ShowStatus($"New Game Failed: {e.Message}", Color.red);
            Debug.LogError($"New game error: {e}");
        }
    }
    
    public void DeleteSave()
    {
        try
        {
            GameLoader.Instance.DeleteSaveData();
            ShowStatus("Save Data Deleted!", Color.yellow);
            UpdateSaveInfo();
        }
        catch (Exception e)
        {
            ShowStatus($"Delete Failed: {e.Message}", Color.red);
            Debug.LogError($"Delete error: {e}");
        }
    }
    
    public void QuickSave()
    {
        SaveGame();
    }
    
    public void QuickLoad()
    {
        LoadGame();
    }
    
    void ShowStatus(string message, Color color)
    {
        if (saveStatusText != null)
        {
            saveStatusText.text = message;
            saveStatusText.color = color;
            saveStatusText.gameObject.SetActive(true);
            
            // Hide status after 3 seconds
            Invoke("HideStatus", 3f);
        }
        
        Debug.Log(message);
    }
    
    void HideStatus()
    {
        if (saveStatusText != null)
        {
            saveStatusText.gameObject.SetActive(false);
        }
    }
    
    public void RefreshSaveInfo()
    {
        UpdateSaveInfo();
    }
    
    // Called from main menu
    public void ContinueGame()
    {
        if (GameLoader.Instance.HasSaveData())
        {
            LoadGame();
        }
        else
        {
            StartNewGame();
        }
    }
    
    // Auto-save notification
    public void ShowAutoSaveNotification()
    {
        ShowStatus("Auto-save completed", Color.cyan);
    }
}

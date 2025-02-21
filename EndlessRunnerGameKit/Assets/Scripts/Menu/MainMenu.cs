using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class MainMenu : Panel
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button logoutButton = null;
    [SerializeField] private Button gameButton = null;
    [SerializeField] private Button leaderboardsButton = null;
    [SerializeField] private Button renameButton = null;
    [SerializeField] private string whichScene;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        logoutButton.onClick.AddListener(SignOut);
        gameButton.onClick.AddListener(StartGame);
        leaderboardsButton.onClick.AddListener(Leaderboards);
        renameButton.onClick.AddListener(RenamePlayer);
        base.Initialize();
    }
    
    public override void Open()
    {
        UpdatePlayerNameUI();
        base.Open();
    }
    
    private void SignOut()
    {
        MenuManager.Singleton.SignOut();
    }
    
    private void UpdatePlayerNameUI()
    {
        nameText.text = AuthenticationService.Instance.PlayerName;
    }

    private void StartGame()
    {
        SceneManager.LoadScene(whichScene);
    }
    
    private void Leaderboards()
    {
        PanelManager.Open("leaderboards");
    }

    private void RenamePlayer()
    {
        GetInputMenu panel = (GetInputMenu)PanelManager.GetSingleton("input");
        panel.Open(RenamePlayerConfirm, GetInputMenu.Type.String, 20, "Enter a new name:", "Send", "Cancel");
    }

    private async void RenamePlayerConfirm(string input)
    {
        renameButton.interactable = false;
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(input);
            UpdatePlayerNameUI();
        }
        catch
        {
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open(ErrorMenu.Action.None, "Fail to change the name.", "OK");
        }
        renameButton.interactable = true;
    }
}
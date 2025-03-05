using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;

public class LeaderboardsMenu : Panel
{

    [SerializeField] private int playersPerPage = 10;
    [SerializeField] private LeaderboardsPlayerItem playerItemPrefab = null;
    [SerializeField] private RectTransform playersContainer = null;
    [SerializeField] public TextMeshProUGUI pageText = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button addScoreButton = null;
    [SerializeField] private TextMeshProUGUI playerRankText = null;
    [SerializeField] private TextMeshProUGUI playerScoreText = null;

    private int currentPage = 1;
    private int totalPages = 0;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        ClearPlayersList();
        closeButton.onClick.AddListener(ClosePanel);
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
        addScoreButton.onClick.AddListener(AddScore);
        base.Initialize();
    }
    
    public override void Open()
    {
        pageText.text = "-";
        nextButton.interactable = false;
        prevButton.interactable = false;
        base.Open();
        ClearPlayersList();
        currentPage = 1;
        totalPages = 0;
        AddScoreAsync(0);
        LoadPlayers(1);
    }
    
    private void AddScore()
    {
        AddScoreAsync(0);
    }
    
    public async void AddScoreAsync(int score)
    {
        addScoreButton.interactable = false;
        try
        {
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("dreamrunner2025", score);
            LoadPlayers(currentPage);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        addScoreButton.interactable = true;
    }

    private async void LoadPlayers(int page)
    {
        nextButton.interactable = false;
        prevButton.interactable = false;
        try
        {
            GetScoresOptions options = new GetScoresOptions();
            options.Offset = (page - 1) * playersPerPage;
            options.Limit = playersPerPage;
            var scores = await LeaderboardsService.Instance.GetScoresAsync("dreamrunner2025", options);
            var playerscore = await LeaderboardsService.Instance.GetPlayerScoreAsync("dreamrunner2025");
            ClearPlayersList();
            for (int i = 0; i < scores.Results.Count; i++)
            {
                Vector2 spawnPosition = new Vector2(0, -75-i * 150); 
                LeaderboardsPlayerItem item = Instantiate(playerItemPrefab, playersContainer);
                item.Initialize(scores.Results[i]);
                RectTransform rectTransform = item.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = spawnPosition;
            }
            playerRankText.text = "Your rank: " + (playerscore.Rank + 1).ToString();
            playerScoreText.text = "Score: " + (playerscore.Score).ToString();
            totalPages = Mathf.CeilToInt((float)scores.Total / (float)scores.Limit);
            currentPage = page;
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        pageText.text = currentPage.ToString() + "/" + totalPages.ToString();
        nextButton.interactable = currentPage < totalPages && totalPages > 1;
        prevButton.interactable = currentPage > 1 && totalPages > 1;
    }

    private void NextPage()
    {
        if (currentPage + 1 > totalPages)
        {
            LoadPlayers(1);
        }
        else
        {
            LoadPlayers(currentPage + 1);
        }
    }

    private void PrevPage()
    {
        if (currentPage - 1 <= 0)
        {
            LoadPlayers(totalPages);
        }
        else
        {
            LoadPlayers(currentPage - 1);
        }
    }

    private void ClosePanel()
    {
        Close();
    }

    private void ClearPlayersList()
    {
        LeaderboardsPlayerItem[] items = playersContainer.GetComponentsInChildren<LeaderboardsPlayerItem>();
        if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Destroy(items[i].gameObject);
            }
        }
    }

}
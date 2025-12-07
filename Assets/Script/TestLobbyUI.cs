using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestLobbyUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TestLobby testLobby;

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_Text outputText;

    private void Awake()
    {
        if (testLobby == null)
            Debug.LogError("TestLobby reference missing on UI.");
    }


    public void OnCreateLobbyPressed()
    {
        testLobby.CreateLobby();
        AppendOutput("Creating lobby...");
    }

    public void OnListLobbiesPressed()
    {
        testLobby.ListLobbies();
        AppendOutput("Listing lobbies...");
    }

    public void OnJoinLobbyPressed()
    {
        string code = joinCodeInput.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            AppendOutput("Join code is empty!");
            return;
        }

        testLobby.JoinLobby(code);
        AppendOutput("Joining lobby with code: " + code);
    }

    public void OnUpdatePlayerNamePressed()
    {
        string newName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            AppendOutput("Player name cannot be empty!");
            return;
        }

        testLobby.UpdatePlayerName(newName);
        AppendOutput("Updating player name to: " + newName);
    }

 
    private void AppendOutput(string msg)
    {
        outputText.text += msg + "\n";
    }
}

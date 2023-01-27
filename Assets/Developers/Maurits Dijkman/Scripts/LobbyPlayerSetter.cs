using TMPro;
using UnityEngine;

public class LobbyPlayerSetter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerReady;
    [HideInInspector] public bool isAssigned = false;
    private bool isReady = false;

    private void Start()
    {
        ResetState();
    }

    public void SetPlayerName(string pPlayerName, bool setAssignState = true)
    {
        if (setAssignState)
            SetAssignedState(true);

        playerName.text = pPlayerName;
    }

    public void SetPlayerReadyState(bool pIsReady, bool setAssignState = true)
    {
        if (setAssignState)
            SetAssignedState(true);

        isReady = pIsReady;

        if (pIsReady)
            playerReady.text = $"<color=green>Ready</color>";
        else
            playerReady.text = $"<color=red>Not Ready</color>";
    }

    private void SetAssignedState(bool pIsAssigned)
    {
        isAssigned = pIsAssigned;
    }

    public void ResetState()
    {
        SetAssignedState(false);
        SetPlayerName("Waiting for player to join...", false);
        SetPlayerReadyState(false, false);
    }

    public string GetPlayerName()
    {
        return playerName.text;
    }

    public bool GetReadyState()
    {
        return isReady;
    }
}

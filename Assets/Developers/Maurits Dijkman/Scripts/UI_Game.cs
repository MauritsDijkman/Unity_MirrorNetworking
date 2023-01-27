using TMPro;
using UnityEngine;

public class UI_Game : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text msText = null;

    private void Awake()
    {
        if (msText == null)
            msText = GetComponentInChildren<TMP_Text>();
    }

    public void SetText(string pText)
    {
        if (msText != null)
            msText.text = $"{pText}";
    }

    public void LeaveServer()
    {
        foreach (PlayerHandler player in FindObjectsOfType<PlayerHandler>())
        {
            if (player.isLocalPlayer)
                player.StopClient();
        }
    }
}

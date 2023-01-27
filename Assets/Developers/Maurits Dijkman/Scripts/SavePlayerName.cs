using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePlayerName : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject panelName = null;
    [SerializeField] private GameObject panelServer = null;

    [Header("Name InputField")]
    [SerializeField] private TMP_InputField nameInputField = null;

    [Header("Button")]
    [SerializeField] private Button confirmButton = null;

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
            nameInputField.text = PlayerPrefs.GetString("PlayerName");
    }

    public void SaveName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
            PlayerPrefs.DeleteKey("PlayerName");

        PlayerPrefs.SetString("PlayerName", nameInputField.text);

        if (panelName != null && panelName.activeSelf)
            panelName.SetActive(false);
        if (panelServer != null && !panelServer.activeSelf)
            panelServer.SetActive(value: true);
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
            confirmButton.interactable = false;
        else
            confirmButton.interactable = true;
    }
}

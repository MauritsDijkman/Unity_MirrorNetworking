using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseLook : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform body;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 100f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
            enabled = false;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMouseLockState();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        playerCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        body.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void HandleMouseLockState()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Lobby")
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
                Cursor.lockState = CursorLockMode.None;

            if (Cursor.visible)
                Cursor.visible = false;
            else
                Cursor.visible = true;
        }
        else if (SceneManager.GetActiveScene().name == "Lobby")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

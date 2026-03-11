using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("Menus")]
    public GameObject MainMenu;           // Main menu panel
    public GameObject GunCanvas;          // Gun selection panel
    public GameObject ModeCanvas;         // Mode canvas panel
    public GameObject ModeMenu;           // Mode buttons container (inside ModeCanvas)

    [Header("Backgrounds")]
    public GameObject MainMenuBackground;
    public GameObject GunCanvasBackground;
    public GameObject ModeCanvasBackground;

    private int selectedGun = 0;
    private int selectedMode = 0;         // 0 = Escalation, 1 = Evolution

    // ===========================
    // GUN MENU
    // ===========================

    public void OpenGunMenu()
    {
        MainMenu.SetActive(false);
        GunCanvas.SetActive(true);
        GunCanvasBackground.SetActive(true);
        MainMenuBackground.SetActive(false);
    }

    public void BackToMainMenuFromGun()
    {
        MainMenu.SetActive(true);
        GunCanvas.SetActive(false);

        MainMenuBackground.SetActive(true);
        GunCanvasBackground.SetActive(false);
    }

    void OnEnable()
    {
        // Always start with MainMenu active and ModeCanvas inactive
        MainMenu.SetActive(true);
        ModeCanvas.SetActive(false);
        GunCanvas.SetActive(false);

        MainMenuBackground.SetActive(true);
        ModeCanvasBackground.SetActive(false);
        GunCanvasBackground.SetActive(false);

        ModeMenu.SetActive(false);
    }


    public void SelectGun(int gunIndex)
    {
        selectedGun = gunIndex;

        // Save to PlayerPrefs
        PlayerPrefs.SetInt("SelectedGun", selectedGun);

        // Update persistent GameManager if needed
        if (GameManager.Instance != null)
            GameManager.Instance.SelectedGun = (GunController.GunType)selectedGun;

        Debug.Log("Selected gun saved: " + selectedGun);
    }


    // ===========================
    // MODE MENU
    // ===========================

    public void OpenModeMenu()
    {
        MainMenu.SetActive(false);
        ModeCanvas.SetActive(true);
        ModeMenu.SetActive(true);

        MainMenuBackground.SetActive(false);
        ModeCanvasBackground.SetActive(true);
    }

    public void BackToMainMenuFromMode()
    {
        ModeCanvas.SetActive(false);
        ModeMenu.SetActive(false);
        MainMenu.SetActive(true);

        ModeCanvasBackground.SetActive(false);
        MainMenuBackground.SetActive(true);
    }

    public void SelectMode(int modeIndex)
    {
        selectedMode = modeIndex;

        // Save to PlayerPrefs
        PlayerPrefs.SetInt("SelectedMode", selectedMode);

        // Update persistent GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectedMode = (GameMode)selectedMode;
        }

        // Load PlayScene
        UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
    }


    // ===========================
    // PLAY / QUIT
    // ===========================

    public void PlayGame()
    {
        // This is used only if you skip mode selection and go directly to PlayScene
        PlayerPrefs.SetInt("SelectedGun", selectedGun);
        PlayerPrefs.SetInt("SelectedMode", selectedMode);
        SceneManager.LoadScene("PlayScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

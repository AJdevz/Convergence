using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public WaveTextDisplay waveTextDisplay; // Reference to the WaveTextDisplay script
    private bool isPaused = false;
    private bool wasWaveTextActiveBeforePause = false; // Tracks if wave text was visible before pausing

    private AudioSource gameAudio; // Reference to the AudioSource for the game

    void Start()
    {
        pauseMenuUI.SetActive(false);
        gameAudio = Object.FindFirstObjectByType<AudioSource>(); // Find the AudioSource in the scene
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Stop the audio when the game is paused
        if (gameAudio != null)
            gameAudio.Pause();

        // Store whether wave text was active before pausing
        if (waveTextDisplay != null && waveTextDisplay.waveText != null)
        {
            wasWaveTextActiveBeforePause = waveTextDisplay.waveText.gameObject.activeSelf;
            waveTextDisplay.waveText.gameObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Resume the audio when the game is resumed
        if (gameAudio != null)
            gameAudio.UnPause();

        // Only show wave text if it was active before pausing
        if (waveTextDisplay != null && waveTextDisplay.waveText != null)
            waveTextDisplay.waveText.gameObject.SetActive(wasWaveTextActiveBeforePause);
    }

    // Restart the current game scene
    public void RestartGame()
    {
        SaveRunProgress(); // ✅ SAVE BEFORE RESTART

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Go back to the main menu safely
    public void GoToMainMenu()
    {
        SaveRunProgress(); // ✅ SAVE BEFORE EXIT

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectedMode = GameMode.Escalation;
        }

        PlayerPrefs.DeleteKey("SelectedMode");

        SceneManager.LoadScene("MainMenu");
    }

    bool hasSaved = false;

    void SaveRunProgress()
    {
        if (hasSaved) return;

        hasSaved = true;

        CoinsManager.Instance?.AddToTotal();
        GameManager.Instance?.SaveGame();
    }
}

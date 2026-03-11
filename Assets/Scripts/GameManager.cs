using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode { Escalation, Evolution }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton

    [Header("Game Settings")]
    public GameMode SelectedMode = GameMode.Escalation;

    // ✅ For gun persistence
    public GunController.GunType SelectedGun = GunController.GunType.AssaultRifle;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =============================
    // MODE SELECTION METHODS
    // =============================
    public void SetModeEscalation()
    {
        SelectedMode = GameMode.Escalation;
        PlayerPrefs.SetInt("SelectedMode", 0);
        SceneManager.LoadScene("PlayScene");
    }

    public void SetModeEvolution()
    {
        SelectedMode = GameMode.Evolution;
        PlayerPrefs.SetInt("SelectedMode", 1);
        SceneManager.LoadScene("PlayScene");
    }
}

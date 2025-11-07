using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject startMenuUI;      
    public Button startButton;          
    public GameObject gameOverUI;       
    public Button restartButton;        
    public GameObject pauseUI;          
    public Button returnToTitleButton;  

    [Header("Level UI")]
    public TextMeshProUGUI levelText;   

    [HideInInspector] public bool gameStarted = false;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public bool isPaused = false;


    private SpawnManager spawnManager;
    private PlayerController player;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameStarted = false;
        gameOver = false;
        isPaused = false;

        spawnManager = FindFirstObjectByType<SpawnManager>();
        player = FindFirstObjectByType<PlayerController>();

        // Initial UI setup
        if (startMenuUI != null)
            startMenuUI.SetActive(true);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (pauseUI != null)
            pauseUI.SetActive(false);

        if (restartButton != null)
            restartButton.gameObject.SetActive(false);

        if (levelText != null)
        {
            levelText.gameObject.SetActive(false);
            levelText.text = "Level: 1";
        }

        // --- Setup Buttons ---
        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartButtonPressed);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartButtonPressed);
        }

        if (returnToTitleButton != null)
        {
            returnToTitleButton.onClick.RemoveAllListeners();
            returnToTitleButton.onClick.AddListener(OnReturnToTitleButtonPressed);
            returnToTitleButton.gameObject.SetActive(false);
        }

        // Disable spawning until game starts
        if (spawnManager != null)
            spawnManager.enabled = false;
    }

    void Update()
    {
        // Handle pause toggle
        if (gameStarted && !gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // End game if player falls off platform
        if (gameStarted && !gameOver && player != null && player.transform.position.y < -5f)
        {
            EndGame();
        }
    }

    // --- Start Game ---
    private void OnStartButtonPressed()
    {
        gameStarted = true;
        AudioManager.Instance?.PlayBackgroundMusic();
        gameOver = false;

        if (startButton != null)
            startButton.gameObject.SetActive(false);
        if (startMenuUI != null)
            startMenuUI.SetActive(false);

        if (spawnManager != null)
            spawnManager.enabled = true;

        if (levelText != null)
            levelText.gameObject.SetActive(true);

        UpdateLevelText(1);

        Debug.Log(" Game Started!");
    }

    // --- End Game ---
    public void EndGame()
    {
        gameOver = true;
        gameStarted = false;
        
        AudioManager.Instance?.StopBackgroundMusic();
        Debug.Log(" Game Over!");

        if (spawnManager != null)
        {
            spawnManager.StopAllCoroutines();
            spawnManager.enabled = false;
        }

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        Time.timeScale = 1f;
    }

    // --- Restart Game ---
    private void OnRestartButtonPressed()
    {
        Debug.Log(" Restarting Game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- Pause Game ---
    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // ✅ Clear velocity of enemies and player to prevent stored kinetic energy
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if (enemy.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        if (pauseUI != null)
            pauseUI.SetActive(true);

        if (returnToTitleButton != null)
            returnToTitleButton.gameObject.SetActive(true);

        Debug.Log(" Game Paused");
    }



    // --- Resume Game ---
    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseUI != null)
            pauseUI.SetActive(false);

        if (returnToTitleButton != null)
            returnToTitleButton.gameObject.SetActive(false);

        Debug.Log(" Game Resumed");
    }

    // --- Return to Title Screen (acts as restart, but unpauses first) ---
    private void OnReturnToTitleButtonPressed()
    {
        Debug.Log(" Returning to Title (Restarting Scene).");

        // Smooth unpause before reload
        isPaused = false;
        Time.timeScale = 1f;

        // Hide pause UI before reloading
        if (pauseUI != null)
            pauseUI.SetActive(false);
        if (returnToTitleButton != null)
            returnToTitleButton.gameObject.SetActive(false);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- Update Level Text ---
    public void UpdateLevelText(int level)
    {
        if (levelText != null)
            levelText.text = "Level: " + level.ToString();
    }
}

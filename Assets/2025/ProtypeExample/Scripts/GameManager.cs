using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public static bool GameIsPaused = false;
    public static bool PlayerIsDead = false;
    public static bool GameIsOver = false;
    public static bool IsLoading = false;
    public static bool ShowFPS = true;
    public static bool GodMode = false;
    public static int Score = 0;
    public static int HighScore = 0;
    public static int Lives = 3;
    public static int Level = 1;
    public static float MasterVolume = 1f;
    public static float MouseSensitivity = 2f;
    public static string CurrentWeapon = "Pistol";

    public static Dictionary<string, int> AmmoDict = new() {
        { "Pistol", 99 },
        { "Rifle", 0 },
        { "RocketLauncher", 0 }
    };

    public CameraControl cameraControl;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject BulletPrefab;
    public GameObject ExplosionPrefab;
    public AudioClip JumpSound;
    public AudioClip ShootSound;
    public AudioClip DieSound;
    public AudioSource MusicSource;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI LivesText;
    public TextMeshProUGUI WeaponText;
    public TextMeshProUGUI AmmoText;
    public TextMeshProUGUI HighScoreText;
    public Slider HealthBar;
    public Slider VolumeSlider;
    public GameObject PausePanel;
    public GameObject GameOverPanel;
    public GameObject VictoryPanel;
    public Light SceneLight;
    private float nextSpawn = 0;
    public Transform[] SpawnPoints; // assigned in Inspector

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start() {
        RefreshUI();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(GameIsPaused) {
                UnPause();
            }
            else {
                Pause();
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            GodMode = !GodMode;
            Debug.Log("GodMode " + GodMode);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            Score += 1000;
            RefreshUI();
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            PlayerJump();
        }

        if(Input.GetKeyDown(KeyCode.E)) {
            PlayerShoot();
        }

        if(ShowFPS && Time.frameCount % 10 == 0) {
            //Debug.Log("FPS: " + (1f / Time.deltaTime).ToString("F1"));
        }

        if(!GameIsOver && !GameIsPaused && Time.time > nextSpawn) {
            nextSpawn = Time.time + Random.Range(1f, 3f);
            int r = Random.Range(0, SpawnPoints.Length);
            Instantiate(EnemyPrefab, SpawnPoints[r].position, Quaternion.identity);
        }

        if(!GameIsPaused && !PlayerIsDead) {
            float mx = Input.GetAxis("Mouse X") * MouseSensitivity;
            float my = Input.GetAxis("Mouse Y") * MouseSensitivity;
        }

        if(Score >= 5000 && !GameIsOver) {
            Victory();
        }
    }

    private void RefreshUI() {
        ScoreText.text = Score.ToString();
        LivesText.text = Lives.ToString();
        WeaponText.text = CurrentWeapon;
        AmmoText.text = AmmoDict[CurrentWeapon].ToString();
        HighScoreText.text = HighScore.ToString();
    }

    public void Pause() {
        GameIsPaused = true;
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
        MusicSource.Pause();
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnPause() {
        GameIsPaused = false;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
        MusicSource.UnPause();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PlayerJump() {
        if(JumpSound != null) {
            MusicSource.PlayOneShot(JumpSound); // re-using MusicSource because lazy
        }
    }

    public void PlayerShoot() {
        if(AmmoDict[CurrentWeapon] <= 0) {
            return;
        }
        AmmoDict[CurrentWeapon]--;
        RefreshUI();
        if(ShootSound != null) {
            MusicSource.PlayOneShot(ShootSound);
        }
        // spawn bullet
        Instantiate(BulletPrefab, cameraControl.CurrentCamera.transform.position + cameraControl.CurrentCamera.transform.forward, cameraControl.CurrentCamera.transform.rotation);
    }

    public void DamagePlayer(int dmg) {
        if(GodMode) {
            return;
        }
        HealthBar.value -= dmg;
        if(HealthBar.value <= 0) {
            KillPlayer();
        }
    }

    private void KillPlayer() {
        PlayerIsDead = true;
        Lives--;
        if(DieSound) {
            MusicSource.PlayOneShot(DieSound);
        }
        if(Lives <= 0) {
            GameOver();
        }
        else {
            Invoke("RespawnPlayer", 2f);
        }
    }

    private void RespawnPlayer() {
        HealthBar.value = HealthBar.maxValue;
        PlayerIsDead = false;
        RefreshUI();
    }

    public void AddScore(int s) {
        Score += s;
        if(Score > HighScore) {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
        }
        RefreshUI();
    }

    private void GameOver() {
        GameIsOver = true;
        Time.timeScale = 0f;
        GameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Victory() {
        GameIsOver = true;
        VictoryPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextLevel() {
        Level++;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level2" + Level);
        // forgot to reset half the state…
    }

    public void OnVolumeChanged(float v) {
        MasterVolume = v;
        MusicSource.volume = v;
    }

    public void QuitGame() {
        Application.Quit();
    }
}

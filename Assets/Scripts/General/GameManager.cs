using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("HUD")]
    public TMP_Text gameTimeText;      // e.g., HUD_LeftSide/GameTimeText
    public TMP_Text scaredTimerText;   // e.g., HUD_RightSide/RightSide_Stack/ScaredTimerText

    [Header("Scared Timer")]
    public bool showScaredDecimals = true;
    public float scaredTimer = 0f;

    float gameTime = 0f;
    bool gameTimerRunning = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        ResetGameTimer(); // ensure 00:00:00 at scene start
    }

    void Update()
    {
        if (gameTimerRunning)
        {
            gameTime += Time.deltaTime;
            if (gameTimeText) gameTimeText.text = FormatGameTime(gameTime);
        }

        if (scaredTimer > 0f)
        {
            scaredTimer = Mathf.Max(0f, scaredTimer - Time.deltaTime);
            if (scaredTimerText)
            {
                if (!scaredTimerText.gameObject.activeSelf) scaredTimerText.gameObject.SetActive(true);
                scaredTimerText.text = showScaredDecimals ? scaredTimer.ToString("0.0") : Mathf.CeilToInt(scaredTimer).ToString();
            }
        }
        else
        {
            if (scaredTimerText && scaredTimerText.gameObject.activeSelf)
                scaredTimerText.gameObject.SetActive(false);
        }
    }

    public void StartScared(float seconds)
    {
        scaredTimer += Mathf.Max(0f, seconds);
    }

    public void ResetGameTimer()
    {
        gameTime = 0f;
        gameTimerRunning = false;
        if (gameTimeText) gameTimeText.text = "00:00:00";
    }

    public void StartGameTimer() => gameTimerRunning = true;
    public void StopGameTimer()  => gameTimerRunning = false;

    string FormatGameTime(float t)
    {
        int totalSeconds = Mathf.FloorToInt(t);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        int centi   = Mathf.FloorToInt((t - totalSeconds) * 100f); // 0..99
        return $"{minutes:00}:{seconds:00}:{centi:00}";
    }
}

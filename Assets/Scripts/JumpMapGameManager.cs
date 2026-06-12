using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class JumpMapGameManager : MonoBehaviour
{
    public static JumpMapGameManager Instance { get; private set; }

    [SerializeField] private float timeLimitSeconds = 60f;
    [SerializeField] private int scorePerCoin = 10;
    private float remainingTime;
    private int score;
    private bool finished;
    private string resultMessage;

    public int Score => score;
    public float RemainingTime => remainingTime;
    public bool IsFinished => finished;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        remainingTime = timeLimitSeconds;
        CoinPickup.ResetCollectedCount();
    }

    private void Update()
    {
        if (!finished)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                GameOver("Time Over!");
            }
        }

        bool restartByKeyboard = Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
        bool restartByMouse = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && finished;

        if (restartByKeyboard || restartByMouse)
        {
            Restart();
        }
    }

    public void AddCoinScore()
    {
        if (finished)
        {
            return;
        }

        score += scorePerCoin;
    }

    public void ClearGoal()
    {
        if (finished)
        {
            return;
        }

        finished = true;
        Time.timeScale = 0f;
        resultMessage = "CLEAR";
    }

    public void GameOver(string message)
    {
        if (finished)
        {
            return;
        }

        finished = true;
        Time.timeScale = 0f;
        resultMessage = "FAIL";
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnGUI()
    {
        const int width = 250;
        const int height = 74;
        GUIStyle hudStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold
        };

        GUI.Box(new Rect(16, 16, width, height), string.Empty);
        GUI.Label(new Rect(32, 28, width - 32, 28), $"Score: {score}", hudStyle);
        GUI.Label(new Rect(32, 56, width - 32, 28), $"Time: {Mathf.CeilToInt(remainingTime)}", hudStyle);

        if (!finished)
        {
            return;
        }

        int boxWidth = 420;
        int boxHeight = 120;
        float x = (Screen.width - boxWidth) * 0.5f;
        float y = (Screen.height - boxHeight) * 0.5f;
        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.62f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = previousColor;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 72,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        GUI.Label(new Rect(x, y, boxWidth, boxHeight), resultMessage, titleStyle);
    }
}

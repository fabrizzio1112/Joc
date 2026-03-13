using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int health = 3;
    private bool isDead = false;
    private bool hasWon = false;

    private int totalCoins;
    private int collectedCoins = 0;

    private VisualElement root;
    private Label scoreLabel;
    private Label healthLabel;
    private Label gameOverLabel;

    void Start()
    {
        Camera.main.backgroundColor = Color.white;

        root = FindObjectOfType<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("ScoreLabel");
        healthLabel = root.Q<Label>("HealthLabel");
        
        // Buscamos el texto de Game Over y lo ocultamos al inicio
        gameOverLabel = root.Q<Label>("GameOverLabel");
        gameOverLabel.style.display = DisplayStyle.None; 
        
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        UpdateUI();
    }

    public void AddScore(int points)
    {
        if (isDead || hasWon) return;
        score += points;
        UpdateUI();
    }

    public void CoinCollected()
    {
        if (isDead || hasWon) return;
        collectedCoins++;
        if (collectedCoins >= totalCoins && totalCoins > 0)
        {
            Win();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || hasWon) return;
        health -= damage;
        UpdateUI();

        if (health <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        scoreLabel.text = "Puntuación: " + score;
        healthLabel.text = "Vida: " + health;
    }

    void Die()
    {
        isDead = true;
        StartCoroutine(FadeInScreen(
            new Color(1.0f, 1.0f, 1.0f, 0.95f), // Fondo blanco
            new Color(0.7f, 0.0f, 0.0f, 1f),    // Texto rojo oscuro
            "¡FIN DEL JUEGO!"
        ));
        StartCoroutine(SendScoreToServer());
    }

    void Win()
    {
        hasWon = true;
        StartCoroutine(FadeInScreen(
            new Color(0.0f, 0.3f, 0.1f, 0.95f), // Fondo verde oscuro
            new Color(1f, 0.9f, 0.2f, 1f),      // Texto amarillo dorado
            "¡VICTORIA!"
        ));
        StartCoroutine(SendScoreToServer());
    }

    IEnumerator FadeInScreen(Color targetBgColor, Color targetTextColor, string text)
    {
        gameOverLabel.text = text;
        gameOverLabel.style.fontSize = 80;
        gameOverLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        gameOverLabel.style.display = DisplayStyle.Flex;

        float duration = 1.2f; // Duración de la animación (1.2 segundos)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t); // Efecto de transición suave

            // Interpolar el color de fondo desde transparente
            Color bg = targetBgColor;
            bg.a = targetBgColor.a * smoothT;
            root.style.backgroundColor = new StyleColor(bg);

            // Interpolar el color del texto para que aparezca poco a poco
            Color txt = targetTextColor;
            txt.a = smoothT;
            gameOverLabel.style.color = new StyleColor(txt);

            yield return null;
        }

        // Asegurar que terminen en los valores objetivos finales
        root.style.backgroundColor = new StyleColor(targetBgColor);
        gameOverLabel.style.color = new StyleColor(targetTextColor);
    }

    // Petición al servidor de Node
    IEnumerator SendScoreToServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("score", score.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/gameover", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error al conectar con Node: " + www.error);
            }
            else
            {
                Debug.Log("Servidor Node respondió correctamente.");
            }
        }
    }

    // Opcional: Reiniciar juego pulsando la tecla R
    void Update()
    {
        if ((isDead || hasWon) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
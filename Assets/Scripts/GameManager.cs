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

    private Label scoreLabel;
    private Label healthLabel;
    private Label gameOverLabel;

    void Start()
    {
        var root = FindObjectOfType<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("ScoreLabel");
        healthLabel = root.Q<Label>("HealthLabel");
        
        // Buscamos el texto de Game Over y lo ocultamos al inicio
        gameOverLabel = root.Q<Label>("GameOverLabel");
        gameOverLabel.style.display = DisplayStyle.None; 
        
        UpdateUI();
    }

    public void AddScore(int points)
    {
        if (isDead) return;
        score += points;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
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
        gameOverLabel.style.display = DisplayStyle.Flex;
        StartCoroutine(SendScoreToServer());
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
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
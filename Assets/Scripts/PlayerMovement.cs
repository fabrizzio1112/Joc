using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento horizontal
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Voltear el sprite según la dirección
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Enviar velocidad al Animator
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
        }

        // Salto básico
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }
    // Añade esto dentro de la clase PlayerMovement
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager gm = FindFirstObjectByType<GameManager>();

        if (collision.CompareTag("Coin"))
        {
            gm.AddScore(10);
            gm.CoinCollected();
            Destroy(collision.gameObject); // Destruye la moneda
        }
        else if (collision.CompareTag("Spike"))
        {
            gm.TakeDamage(1);
        }
    }
}

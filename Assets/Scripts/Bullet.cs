using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;

    void Start()
    {
        // Peluru langsung hancur otomatis setelah 3 detik jika tidak kena apa-apa
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Bergerak ke arah "depan" peluru (Sumbu X lokal)
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Ambil script EnemyAI (bukan EnemyBoss atau yang lain)
            EnemyAI enemy = collision.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                // Panggil fungsi TakeDamage
                enemy.TakeDamage(damage);
                Debug.Log("Kena Musuh!"); // Cek di Console apakah tulisan ini muncul
            }

            Destroy(gameObject);
        }
    }
}
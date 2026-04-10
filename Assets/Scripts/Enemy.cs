using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackDamage = 10f; // Gunakan float
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private float nextAttackTime;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        var pScript = player.GetComponent<PlayerMovementController_4Direction>();
        if (pScript != null)
        {
            pScript.TakeDamage(attackDamage); // Panggil fungsi di Player
            Debug.Log("Musuh Menyerang!");
        }
    }

}
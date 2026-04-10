using UnityEngine;

[CreateAssetMenu(fileName = "NewStats", menuName = "Stats/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public float moveSpeed = 3f;
    public float maxHealth = 100f;
    public float currentHealth; // Harus float agar sinkron

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
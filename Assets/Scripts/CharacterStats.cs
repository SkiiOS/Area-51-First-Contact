using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [Header("Character Stats")]
    public float moveSpeed = 3f;
    public int maxHealth = 100;

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;
        maxHealth = Mathf.Max(maxHealth, 0);
        Debug.Log($"{damage} damage taken. Remaining health: {maxHealth}");
    }
}

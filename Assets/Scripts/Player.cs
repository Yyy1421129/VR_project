using System;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Transform head;
    [SerializeField] TextMeshProUGUI healthText;

    float health;
    bool isDead;

    public float Health => health;
    public bool IsDead => isDead;

    public event Action OnPlayerDied;

    void Awake()
    {
        health = maxHealth;

        if (healthText == null)
        {
            var healthLabel = GameObject.Find("healthText");
            if (healthLabel != null)
            {
                healthText = healthLabel.GetComponent<TextMeshProUGUI>();
            }
        }

        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        health = Mathf.Max(0f, health - damage);
        UpdateHealthUI();
        Debug.Log($"Player health: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    public void ResetHealth()
    {
        isDead = false;
        health = maxHealth;
        UpdateHealthUI();
    }

    public Vector3 GetHeadPosition()
    {
        return head != null ? head.position : transform.position;
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        OnPlayerDied?.Invoke();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText == null)
        {
            return;
        }

        int currentHp = Mathf.CeilToInt(health);
        int maxHp = Mathf.CeilToInt(maxHealth);
        healthText.text = $"HP: {currentHp} / {maxHp}";

        if (currentHp <= maxHp * 0.3f)
        {
            healthText.color = new Color(1f, 0.35f, 0.35f);
        }
        else if (currentHp <= maxHp * 0.6f)
        {
            healthText.color = new Color(1f, 0.85f, 0.3f);
        }
        else
        {
            healthText.color = Color.white;
        }
    }
}

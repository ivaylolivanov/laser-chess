using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
    private int maxHealth;
    private int currentHealth;

    private Slider slider;

    void Awake() {
        slider = GetComponent<Slider>();
    }

    public void TakeDamage(int damage) {
        if(damage < 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(slider != null) {
            slider.value = currentHealth;
        }

        if(currentHealth <= 0) {
            Die();
        }
    }

    public void SetMaxHealth(int maxHealth) {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;

        if(slider != null)
        {
            slider.maxValue = this.maxHealth;
            slider.value = this.maxHealth;
        }
    }

    private void Die() {
        Destroy(transform.parent.parent.gameObject);
    }
}

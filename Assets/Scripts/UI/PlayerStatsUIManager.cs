using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerStatsUIManager : MonoBehaviour
{
    private static PlayerStatsUIManager instance;
    
    [SerializeField] private Image healthBar, manaBar;
    [SerializeField] private TextMeshProUGUI healthNumbers, manaNumbers;
    
    // not enough mana (temp)
    //[SerializeField] private TextMeshProUGUI noManaText;

    private float currentHealth, currentMana, maxHealth, maxMana;

    private PlayerStatsUIManager()
    {
        instance = this;
    }

    public static PlayerStatsUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerStatsUIManager();
            }

            return instance;
        }
    }

    private void Start()
    {
        // Set health and mana values in bar image
        currentHealth = PlayerStatsManager.Instance.GetCurrentHealth();
        currentMana = PlayerStatsManager.Instance.GetCurrentMana();
        maxHealth = currentHealth;
        maxMana = currentMana;

        // Set health and mana values for text
        healthNumbers.text = currentHealth.ToString() + " / " + maxHealth;
        manaNumbers.text = currentMana.ToString() + " / " + maxMana;

        //noManaText.text = ". ";
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        healthNumbers.text = currentHealth.ToString() + " / " + maxHealth;
        
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    
    public void UseMana(float manaCost)
    {
        if (currentMana - manaCost < 0)
        {
            return;
        }
        currentMana -= manaCost;
        
        manaNumbers.text = currentMana.ToString() + " / " + maxMana;
        
        manaBar.fillAmount = currentMana / maxMana;
    }

    public float GetCurrentMana()
    {
        return currentMana;
    }

    public void Heal(float healingAmount)
    {
        currentHealth += healingAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        
        healthNumbers.text = currentHealth.ToString() + " / " + maxHealth;

        healthBar.fillAmount = currentHealth / maxHealth;
    }
    
    public void RestoreMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, 100);
        
        manaNumbers.text = currentMana.ToString() + " / " + maxMana;

        manaBar.fillAmount = currentMana / maxMana;
    }
    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(1);
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            Heal(1);
        }
    }
}

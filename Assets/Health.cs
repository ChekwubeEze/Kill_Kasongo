using UnityEngine;

public class Health : MonoBehaviour
{
    private int _maxHealth;
    private int _currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _maxHealth = _currentHealth;
    }

    // Update is called once per frame
    void TakeDamage(int _damageAmount)
    {
        _currentHealth -= _damageAmount;
        if (_currentHealth < 0 ) { _currentHealth = 0; }

    }
}

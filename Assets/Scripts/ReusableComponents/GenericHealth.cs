using UnityEngine;

public class GenericHealth : MonoBehaviour
{
    public FloatValue maxHealth;
    
    public float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth.RuntimeValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Heal(float amountToHeal)
    {
        currentHealth += amountToHeal;
        if(currentHealth > maxHealth.RuntimeValue)
        {
            currentHealth = maxHealth.RuntimeValue;
        }
    }

    public virtual void fullHeal()
    {
        currentHealth = maxHealth.RuntimeValue;
    }

    public virtual void damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public virtual void InstaKill()
    {
        currentHealth = 0;
    }
}

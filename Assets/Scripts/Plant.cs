using UnityEngine;

public enum PlantState {Healthy, Infected, Dead}
public class Plant : MonoBehaviour
{
    public PlantState currentState = PlantState.Healthy;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer; 
    public Color healthyColor = new(0.72f, 0.88f, 0.83f); // #B8E2D4
    public Color infectedColor = new(0.83f, 0.23f, 0.41f); // #D53C6A
    public Color deadColor = new(0.19f, 0.02f, 0.11f);     // #31051E

    public void Init()
    {
        UpdateVisuals();
    }
    public void UpdateVisuals()
    {
        if (currentState == PlantState.Healthy) spriteRenderer.color = healthyColor;
        if (currentState == PlantState.Infected) spriteRenderer.color = infectedColor;
        if (currentState == PlantState.Dead) spriteRenderer.color = deadColor;
    }

    public void TakeDamage()
    {
        if (currentState == PlantState.Infected)
        {
            Purge();
        }
        else if (currentState == PlantState.Healthy)
        {
            Die();
        }
    }
    
    void Purge()
    {
        currentState = PlantState.Healthy;
        UpdateVisuals();

        // Play cleanse SFX
    }

    void Die()
    {
        currentState = PlantState.Dead;
        UpdateVisuals();

        // Play death SFX
    }
    
}

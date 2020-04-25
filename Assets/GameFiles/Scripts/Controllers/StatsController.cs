using UnityEngine;

public class StatsController : MonoBehaviour
{
    //Public fields
    public string displayName;
    public float maxHealth = 100;
    public float health = 100;
    public float energy;
    public float maxEnergy = 100;
    [HideInInspector]
    public float score;
    [HideInInspector]
    public bool isOutOflevel;

    //Private fields
    private bool dead;

    // Unity methods
    void LateUpdate()
    {
        if (isOutOflevel)
        {
            return;
        }
        Bounds levelBounds = GameManager.INSTANCE.levelBounds;
        Bounds bounds = GetMaxBounds();
        if (!(levelBounds.Contains(bounds.min) && levelBounds.Contains(bounds.max)))
        {
            isOutOflevel = true;
        }
    }

    // Custom methods
    public void CollectEnergy(float energy)
    {
        this.energy += energy;
        if (this.energy > this.maxEnergy)
        {
            this.energy = this.maxEnergy;
        }
    }

    public float GetHealthRatio()
    {
        return this.health / this.maxHealth;
    }

    public float GetEnergyRatio()
    {
        return this.energy / this.maxHealth;
    }

    public void CollectHealth(float health)
    {
        this.health += health;
        if (this.health > this.maxHealth)
        {
            this.health = this.maxHealth;
        }
    }

    public void AddScore(float score)
    {
        this.score += score;
    }

    public bool IsEnergyFull()
    {
        return this.energy == this.maxEnergy;
    }

    public bool IsHealthFull()
    {
        return this.health == this.maxHealth;
    }

    //In future can adjust this mehtod to adjust according to difficulty
    public bool IsHealthCritical()
    {
        return this.health <= this.maxHealth / 2;
    }

    public bool IsHealthFine()
    {
        return this.health > this.maxHealth/2;
    }

    public void ConsumeEnergy()
    {
        this.energy = 0;
    }

    public void DamageHealth(float damage)
    {
        this.health -= damage;
        if (this.health < 0)
        {
            this.health = 0;
        }
    }

    public Bounds GetMaxBounds()
    {
        var b = new Bounds(transform.position, Vector3.zero);
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    public void Die()
    {
        dead = true;
    }

    public bool IsAlive()
    {
        return !dead;
    }
}

using UnityEngine;

public class StatsController : MonoBehaviour
{
    //Public fields
    public string displayName;
    [HideInInspector]
    public int maxHealth = 100;
    [HideInInspector]
    public int health = 100;
    [HideInInspector]
    public int energy;
    [HideInInspector]
    public int maxEnergy = 100;
    [HideInInspector]
    public int score;
    [HideInInspector]
    public bool isOutOflevel;

    private bool dead;

    // Unity methods
    void LateUpdate()
    {
        if (!isOutOflevel)
        {
            Bounds levelBounds = GameManager.INSTANCE.levelBounds;
            Bounds bounds = GetMaxBounds();
            if (!(levelBounds.Contains(bounds.min) && levelBounds.Contains(bounds.max)))
            {
                isOutOflevel = true;
            }
        }

    }

    // Custom methods
    public void CollectEnergy(int energy)
    {
        if (this.energy == this.maxEnergy)
        {
            return;
        }
        this.energy += energy;
    }

    public void CollectHealth(int health)
    {
        if (this.health == this.maxHealth)
        {
            return;
        }
        this.health += health;
    }

    public void AddScore(int score)
    {
        this.score += score;
    }

    public bool IsEnergyFull()
    {
        return this.energy == this.maxEnergy;
    }

    public void ConsumeEnergy()
    {
        this.energy = 0;
    }

    public void DamageHealth(int damage)
    {
        if (this.health == 0)
        {
            return;
        }
        this.health -= damage;
    }

    private Bounds GetMaxBounds()
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

    public bool isAlive()
    {
        return !dead;
    }
}

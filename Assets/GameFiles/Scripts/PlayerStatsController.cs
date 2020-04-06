﻿using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{

    //Private fields
    private float _maxHealth;
    private float _health;
    private float _energy;
    private float _maxEnergy;
    private int _score;

    //Getters and Setters
    public float health { get { return _health; } }
    public float energy { get { return _energy; } }
    public float maxHealth { get { return _maxHealth; } }
    public float maxEnergy { get { return _maxEnergy; } }
    public int score { get { return _score; } }

    // Unity methods
    void Start()
    {
        _maxEnergy = 100;
        _maxHealth = 100;
        _health = 100;
        _energy = 0;
    }

    // Custom methods
    public void UpdateEnergy(int energy)
    {
        if (_energy == _maxEnergy)
        {
            return;
        }
        _energy += energy;
    }

    public void UpdateHealth(int health)
    {
        if (_health == _maxHealth)
        {
            return;
        }
        _health += health;
    }

    public void UpdateScore(int score)
    {
        _score += score;
    }

    public bool EnergyFull()
    {
        return _energy == _maxEnergy;
    }

    public void ConsumeEnergy()
    {
        _energy = 0;
    }

}
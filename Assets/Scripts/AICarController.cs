using UnityEngine;

public class AICarController : MonoBehaviour
{
    //Public fields
    public string id;

    //Private fields
    private Rigidbody _rigidbody;
    private int _score;
    private int _health;
    private int _energy;

    //Getters and Setters
    new public Rigidbody rigidbody { get { return _rigidbody; } }
    public int score { get { return _score; } }
    public int health { get { return _health; } }
    public int energy { get { return _energy; } }

    //Unity methods
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    //Custom methods
    public void DamageHealth(int damage)
    {
        _health -= damage;
    }
}

using UnityEngine;

public class AICarController : MonoBehaviour
{
    //Public fields
    public string id;

    //Private fields
    private Rigidbody _rigidbody;
    private int _score;
    private int _health;

    //Getters and Setters
    new public Rigidbody rigidbody { get { return _rigidbody; } }
    public int score { get { return _score; } }
    public int health { get { return _health; } }

    //Unity methods
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }
}

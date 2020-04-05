using UnityEngine;

public class AICar : MonoBehaviour
{
    //Public ones
    public string id;
    //Private ones
    private Rigidbody _rigidbody;
    private int _score;
    private int _health;
    //Getters and Setters
    new public Rigidbody rigidbody { get { return _rigidbody; } }
    public int score { get { return _score; } }
    public int health { get { return _health; } }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }
}

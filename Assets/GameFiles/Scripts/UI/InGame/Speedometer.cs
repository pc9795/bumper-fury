using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    //TODO: move it to the player script once speed clamping is implemented.
    public int topSpeed = 40;

    //Private fields
    private Rigidbody playerRigidbody;
    private Text text; //Should be applied to a text element
    private float minEngineSoundPitch = 0.2f;

    //Unity methods
    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (playerRigidbody == null)
        {
            GameObject player = GameManager.INSTANCE.GetPlayer();
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
        text.text = "" + (int)playerRigidbody.velocity.magnitude + " MPH";

        float speedRatio = playerRigidbody.velocity.magnitude / topSpeed + minEngineSoundPitch;
        AudioManager.INSTANCE.GetSound("Engine").source.pitch = speedRatio;
    }
}

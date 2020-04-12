using UnityEngine;

public class Item : MonoBehaviour
{
    [System.Serializable]
    public enum ItemType
    {
        HEALTH_BOOST, ENERGY_BOOST, SPEED_BOOST
    }

    public ItemType type;
    public float groundLevel = 0.5f;
    public float value = 10;
    public float duration = 5;
}

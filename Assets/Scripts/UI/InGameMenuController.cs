using UnityEngine;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{
    //Public fields
    public Image healthBar;
    public Image energyBar;

    //Private fields
    private PlayerStatsController playerStats;
    private GameObject player;

    //Unity methods
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStatsController>();
    }

    void Update()
    {
        energyBar.fillAmount = playerStats.energy / playerStats.maxEnergy;
        healthBar.fillAmount = playerStats.health / playerStats.maxHealth;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    //Public fields
    public Image healthBar;
    public Image energyBar;

    //Private fields
    private StatsController playerStats;

    //Unity methods
    void Update()
    {
        if (playerStats == null)
        {
            InitFromGameManager();
            return;
        }
        //Need values in 0 - 1 scale.
        energyBar.fillAmount = playerStats.energy / playerStats.maxEnergy;
        healthBar.fillAmount = playerStats.health / playerStats.maxHealth;
    }

    //Custom Methods
    void InitFromGameManager()
    {
        GameObject player = GameManager.INSTANCE.GetPlayer();
        //Player is not initialized yet. Earyly Exit
        if (player == null)
        {
            return;
        }
        playerStats = player.GetComponent<StatsController>();
    }
}

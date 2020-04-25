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
            GameObject player = GameManager.INSTANCE.GetPlayer();
            playerStats = player.GetComponent<StatsController>();
        }
        //Need values in 0 - 1 scale.
        energyBar.fillAmount = playerStats.GetEnergyRatio();
        healthBar.fillAmount = playerStats.GetHealthRatio();
    }
}

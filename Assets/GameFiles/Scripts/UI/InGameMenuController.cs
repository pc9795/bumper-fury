using UnityEngine;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{
    //Public fields
    public Image healthBar;
    public Image energyBar;

    //Private fields
    private PlayerStatsController playerStats;

    //Unity methods
    void Update()
    {
        if (playerStats == null)
        {
            InitFromGameManager();
            return;
        }
        energyBar.fillAmount = playerStats.energy / playerStats.maxEnergy;
        healthBar.fillAmount = playerStats.health / playerStats.maxHealth;
    }

    //Custom Methods
    void InitFromGameManager()
    {
        GameObject player = GameManager.INSTANCE.GetPlayer();
        if (player == null)
        {
            return;
        }
        playerStats = player.GetComponent<PlayerStatsController>();
    }


}

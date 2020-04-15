using UnityEngine;

public class GameEnding : MonoBehaviour
{
     //Custom methods
    public void QuitToMainMenu()
    {
        GameManager.INSTANCE.MainMenu();
    }
}

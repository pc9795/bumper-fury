using UnityEngine;

public class ElementalSelector : MonoBehaviour
{
    //Private fields
    private GameObject selectedElementalInstance;
    private bool elementalChanged;
    private bool active;

    //Unity methods
    void Update()
    {
        if (!active || !elementalChanged)
        {
            return;
        }
        Destroy(selectedElementalInstance);
        selectedElementalInstance = Instantiate(
            GameManager.INSTANCE.elementals[GameManager.INSTANCE.selectedElementalIndex].prefab,
            transform.position, Quaternion.identity);
        elementalChanged = false;
    }

    void OnDestroy()
    {
        if (!selectedElementalInstance)
        {
            return;
        }
        Destroy(selectedElementalInstance);
    }

    //Custom methods
    public void NextElemental()
    {
        GameManager.INSTANCE.selectedElementalIndex++;
        GameManager.INSTANCE.selectedElementalIndex %= GameManager.INSTANCE.elementals.Count;
        elementalChanged = true;
    }
    public void PreviousElemental()
    {
        GameManager.INSTANCE.selectedElementalIndex--;
        GameManager.INSTANCE.selectedElementalIndex =
            GameManager.INSTANCE.selectedElementalIndex < 0 ?
            GameManager.INSTANCE.elementals.Count - 1 : GameManager.INSTANCE.selectedElementalIndex;
        elementalChanged = true;

    }
    public void Activate()
    {
        active = true;
        if (selectedElementalInstance)
        {
            Destroy(selectedElementalInstance);
        }
        GameManager.INSTANCE.selectedElementalIndex = 0;
        selectedElementalInstance = Instantiate(
            GameManager.INSTANCE.elementals[GameManager.INSTANCE.selectedElementalIndex].prefab,
            transform.position, Quaternion.identity);
        selectedElementalInstance.transform.Rotate(new Vector3(0, -260, -0));

    }

    public void Deactivate()
    {
        active = false;
        if (selectedElementalInstance)
        {
            Destroy(selectedElementalInstance);
        }
    }
}

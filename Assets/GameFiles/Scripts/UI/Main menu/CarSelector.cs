using UnityEngine;

public class CarSelector : MonoBehaviour
{
    //Private fields
    private GameObject selectedCarInstance;
    private bool carChanged;
    private bool active;

    //Unity methods
    void Update()
    {
        if (!active)
        {
            return;
        }
        if (selectedCarInstance)
        {
            selectedCarInstance.transform.Rotate(new Vector3(0, 0.1f, 0), Space.Self);
        }
        if (!carChanged)
        {
            return;
        }
        Destroy(selectedCarInstance);
        selectedCarInstance = Instantiate(
            GameManager.INSTANCE.cars[GameManager.INSTANCE.selectedCarIndex].modelPrefab, transform.position, Quaternion.identity);
        selectedCarInstance.transform.Rotate(new Vector3(0, -260, -0));
        carChanged = false;
    }

    void OnDestroy()
    {
        if (!selectedCarInstance)
        {
            return;
        }
        Destroy(selectedCarInstance);
    }

    //Custom methods
    public void NextCar()
    {
        GameManager.INSTANCE.selectedCarIndex++;
        GameManager.INSTANCE.selectedCarIndex %= GameManager.INSTANCE.cars.Count;
        carChanged = true;
    }
    public void PreviousCar()
    {
        GameManager.INSTANCE.selectedCarIndex--;
        GameManager.INSTANCE.selectedCarIndex =
            GameManager.INSTANCE.selectedCarIndex < 0 ? 
            GameManager.INSTANCE.cars.Count - 1 : GameManager.INSTANCE.selectedCarIndex;
        carChanged = true;

    }
    public void Activate()
    {
        active = true;
        if (selectedCarInstance)
        {
            Destroy(selectedCarInstance);
        }
        GameManager.INSTANCE.selectedCarIndex = 0;
        selectedCarInstance = Instantiate(
            GameManager.INSTANCE.cars[GameManager.INSTANCE.selectedCarIndex].modelPrefab, transform.position, Quaternion.identity);
        selectedCarInstance.transform.Rotate(new Vector3(0, -260, -0));

    }

    public void Deactivate()
    {
        active = false;
        if (selectedCarInstance)
        {
            Destroy(selectedCarInstance);
        }
    }

}

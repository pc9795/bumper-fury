using System.Collections.Generic;
using UnityEngine;

public class CarSelector : MonoBehaviour
{
    [System.Serializable]
    public class Car
    {
        public GameObject prefab;
        public string name;
    }
    //Public fields
    public List<Car> cars = new List<Car>();
    public int selectedIndex = 0;

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
            selectedCarInstance.transform.Rotate(new Vector3(0, 0.3f, 0), Space.Self);
        }
        if (!carChanged)
        {
            return;
        }
        Destroy(selectedCarInstance);
        selectedCarInstance = Instantiate(cars[selectedIndex].prefab, transform.position, Quaternion.identity);
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
        selectedIndex++;
        selectedIndex %= cars.Count;
        carChanged = true;
    }
    public void PreviousCar()
    {
        selectedIndex--;
        selectedIndex = selectedIndex < 0 ? 0 : selectedIndex;
        carChanged = true;

    }
    public void SelectCar()
    {
        GameManager.INSTANCE.selectedCarName = cars[selectedIndex].name;
    }

    public void Activate()
    {
        active = true;
        if (selectedCarInstance)
        {
            Destroy(selectedCarInstance);
        }
        selectedIndex = 0;
        selectedCarInstance = Instantiate(cars[selectedIndex].prefab, transform.position, Quaternion.identity);
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

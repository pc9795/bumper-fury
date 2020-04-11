using System.Collections.Generic;
using UnityEngine;

public class ElementalSelector : MonoBehaviour
{
    [System.Serializable]
    public class Elemental
    {
        public GameObject prefab;
        public string name;
    }
    //Public fields
    public List<Elemental> elementals = new List<Elemental>();
    public int selectedIndex = 0;

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
        selectedElementalInstance = Instantiate(elementals[selectedIndex].prefab, transform.position, Quaternion.identity);
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
        selectedIndex++;
        selectedIndex %= elementals.Count;
        elementalChanged = true;
    }
    public void PreviousElemental()
    {
        selectedIndex--;
        selectedIndex = selectedIndex < 0 ? 0 : selectedIndex;
        elementalChanged = true;

    }
    public void SelectElemental()
    {
        GameManager.INSTANCE.selectedElementName = elementals[selectedIndex].name;
    }

    public void Activate()
    {
        active = true;
        if (selectedElementalInstance)
        {
            Destroy(selectedElementalInstance);
        }
        selectedIndex = 0;
        selectedElementalInstance = Instantiate(elementals[selectedIndex].prefab, transform.position, Quaternion.identity);
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

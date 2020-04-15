using UnityEngine;
using System.Collections.Generic;

public class WaypointGizmo : MonoBehaviour
{
    public Color color;

    void OnDrawGizmos()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        Gizmos.color = color;
        List<Transform> transforms = new List<Transform>();

        foreach (Transform child in children)
        {
            if (child == this.transform)
            {
                continue;
            }
            transforms.Add(child);
        }

        for (int i = 0; i < transforms.Count; i++)
        {
            if (transforms[i] == transform)
            {
                continue;
            }
            Vector3 curr = transforms[i].position;
            Vector3 prev = Vector3.zero;
            if (i > 0)
            {
                prev = transforms[i - 1].position;
            }
            else if (i == 0 && transforms.Count > 1)
            {
                prev = transforms[transforms.Count - 1].position;
            }
            Gizmos.DrawLine(prev, curr);
            Gizmos.DrawWireSphere(curr, 0.3f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public Transform[] points;
    public List<Transform> points = new List<Transform>();
    void Start()
    {
        //GameObject spawnPointGroupObject = GameObject.Find("SpawnPointGroup");
        //if (spawnPointGroupObject != null)
        //{
        //    Transform spawnPointGroup = spawnPointGroupObject.GetComponent<Transform>();
        //    points = spawnPointGroup.GetComponentsInChildren<Transform>();
        //}
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        //points = spawnPointGroup?.GetComponentsInChildren<Transform>();
        //spawnPointGroup?.GetComponentsInChildren<Transform>(points);
        foreach (Transform point in spawnPointGroup)
        {
            points.Add(point);
        }
    }
}

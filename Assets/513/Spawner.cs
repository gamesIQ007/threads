using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private int amount;
    [SerializeField] private Rect rect;

    private void Awake()
    {
        Unit.AllUnits = new Unit[amount];

        for (int i = 0; i < amount; i++)
        {
            float x = Random.Range(rect.x, rect.width);
            float z = Random.Range(rect.y, rect.height);

            Unit.AllUnits[i] = Instantiate(unitPrefab, new Vector3(x, 0, z), Quaternion.identity);
        }
    }
}

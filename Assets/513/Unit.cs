using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static Unit[] AllUnits;

    public float SatisfactionLevel;
    /*
    private void Update()
    {
        SatisfactionLevel = Mathf.Sqrt(Mathf.Pow(CityStats.BuildsAmount * CityStats.SocialPaymentsAmount, 3)) + Random.Range(0, 30);
    }*/
}

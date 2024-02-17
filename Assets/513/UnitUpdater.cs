using System.Threading;
using UnityEngine;

public class UnitUpdater : MonoBehaviour
{
    [SerializeField] private float[] satisfactionLevels;

    [SerializeField] private bool updateInMainThread;
    [SerializeField] private bool updateInAdditionalThread;

    private Thread unitUpdateThread;
    private bool isEnabled = false;
    private bool isUpdateSatisfactionLevel = false;

    private void Start()
    {
        satisfactionLevels = new float[Unit.AllUnits.Length];

        for (int i = 0; i < satisfactionLevels.Length; i++)
        {
            satisfactionLevels[i] = Unit.AllUnits[i].SatisfactionLevel;
        }

        unitUpdateThread = new Thread(UpdateUnitThread);
        unitUpdateThread.Name = "Unit Update Thread";
        unitUpdateThread.Start();
        isEnabled = true;
    }

    private void Update()
    {
        if (updateInMainThread)
        {
            UpdateUnitSatisfaction(CityStats.BuildsAmount, CityStats.SocialPaymentsAmount);

            Debug.Log("Полезная нагрузка");
        }

        if (isUpdateSatisfactionLevel)
        {
            Debug.Log("Полезная нагрузка");
        }
    }

    private void LateUpdate()
    {
        isUpdateSatisfactionLevel = false;
    }

    private void OnDestroy()
    {
        isEnabled = false;
        unitUpdateThread.Abort();
    }

    private void UpdateUnitSatisfaction(int buildAmound, int socialPayment)
    {
        System.Random rnd = new();

        for (int i = 0; i < satisfactionLevels.Length; i++)
        {
            satisfactionLevels[i] = Mathf.Sqrt(Mathf.Pow(buildAmound * socialPayment, 3)) + rnd.Next(0, 30);
        }
    }

    private void UpdateUnitThread()
    {
        while (isEnabled)
        {
            Debug.Log(Thread.CurrentThread.Name + " work");

            while (updateInAdditionalThread)
            {
                if (isUpdateSatisfactionLevel == false)
                {
                    UpdateUnitSatisfaction(CityStats.BuildsAmount, CityStats.SocialPaymentsAmount);

                    isUpdateSatisfactionLevel = true;
                }
            }
        }
    }
}

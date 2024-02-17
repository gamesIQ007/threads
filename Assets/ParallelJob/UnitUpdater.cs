using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ParallelJob
{
    public class UnitUpdater : MonoBehaviour
    {
        [SerializeField] private float[] satisfactionLevels;

        private NativeArray<float> satisfactionLevelsNative;
        private NativeArray<int> statsNative;
        private NativeArray<float> randomValuesNative;

        private JobHandle jobHandle;

        private void Start()
        {
            satisfactionLevels = new float[Unit.AllUnits.Length];

            for (int i = 0; i < satisfactionLevels.Length; i++)
            {
                satisfactionLevels[i] = Unit.AllUnits[i].SatisfactionLevel;
            }

            satisfactionLevelsNative = new NativeArray<float>(Unit.AllUnits.Length, Allocator.Persistent);
            randomValuesNative = new NativeArray<float>(Unit.AllUnits.Length, Allocator.Persistent);
            statsNative = new NativeArray<int>(2, Allocator.Persistent);
        }

        private void Update()
        {
            // наполнение данными
            statsNative[0] = CityStats.BuildsAmount;
            statsNative[1] = CityStats.SocialPaymentsAmount;

            for (int i = 0; i < Unit.AllUnits.Length; i++)
            {
                randomValuesNative[i] = Random.Range(0, 30);
            }

            // джоб
            UnitUpdateJob unitUpdateJob = new();
            unitUpdateJob.SatisfactionLevels = satisfactionLevelsNative;
            unitUpdateJob.RandomValues = randomValuesNative;
            unitUpdateJob.Stats = statsNative;

            jobHandle = unitUpdateJob.Schedule(Unit.AllUnits.Length, 0); // цифра - количество потоков. 0 - программа сама выберет количество.
        }

        private void LateUpdate()
        {
            jobHandle.Complete();
        }

        private void OnDestroy()
        {
            satisfactionLevelsNative.Dispose();
            randomValuesNative.Dispose();
            statsNative.Dispose();
        }
    }

    [BurstCompile]
    public struct UnitUpdateJob : IJobParallelFor
    {
        [WriteOnly]
        public NativeArray<float> SatisfactionLevels;
        [ReadOnly]
        public NativeArray<int> Stats;
        [ReadOnly]
        public NativeArray<float> RandomValues;

        public void Execute(int index)
        {
            SatisfactionLevels[index] = Mathf.Sqrt(Mathf.Pow(Stats[0] * Stats[1], 3)) + RandomValues[index];
        }
    }
}
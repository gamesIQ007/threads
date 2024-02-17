using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace SimpleTransformJob
{
    public class UnitUpdater : MonoBehaviour
    {
        [SerializeField] private Unit prefab;
        [SerializeField] private int amount;

        private List<Unit> allUnits;

        private NativeArray<Vector3> velocity;
        private NativeArray<float> speeds;
        private TransformAccessArray transformAccessArray;

        private JobHandle moveJobHandle;

        private void Start()
        {
            allUnits = new List<Unit>();

            for (int i = 0; i < amount; i++)
            {
                Unit unit = Instantiate(prefab, new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-4f, 4f)), Quaternion.identity);

                unit.speed = UnityEngine.Random.Range(1f, 2f);
                unit.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, 0) * unit.speed;

                allUnits.Add(unit);
            }

            velocity = new NativeArray<Vector3>(allUnits.Count, Allocator.Persistent);
            speeds = new NativeArray<float>(allUnits.Count, Allocator.Persistent);
            transformAccessArray = new TransformAccessArray(allUnits.Count);

            for (int i = 0; i < allUnits.Count; i++)
            {
                velocity[i] = allUnits[i].velocity;
                speeds[i] = allUnits[i].speed;
                transformAccessArray.Add(allUnits[i].transform);
            }
        }

        private void OnDestroy()
        {
            velocity.Dispose();
            speeds.Dispose();
            transformAccessArray.Dispose();
        }

        private void Update()
        {
            MoveUnitJob moveUnitJob = new();

            moveUnitJob.Velocity = velocity;
            moveUnitJob.Speeds = speeds;
            moveUnitJob.DeltaTime = Time.deltaTime;

            moveJobHandle = moveUnitJob.Schedule(transformAccessArray);
        }

        private void LateUpdate()
        {
            moveJobHandle.Complete();
        }
    }

    [BurstCompile]
    public struct MoveUnitJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Velocity;
        public NativeArray<float> Speeds;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position += Velocity[index] * Speeds[index] * DeltaTime;

            Vector3 dir = Velocity[index].normalized;
            transform.rotation = Quaternion.LookRotation(dir);

            if (transform.position.x > 7)
            {
                Velocity[index] = -Velocity[index];
            }

            if (transform.position.x < -7)
            {
                Velocity[index] = math.abs(Velocity[index]);
            }
        }
    }
}
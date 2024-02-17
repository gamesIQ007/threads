using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace BirdSingle
{
    public partial class UnitUpdater : MonoBehaviour
    {
        [SerializeField] private Unit prefab;
        [SerializeField] private int amount;
        [SerializeField] private float distance;

        private List<Unit> allUnits;

        private NativeArray<Vector3> positionsRead;
        private NativeArray<Vector3> directionsRead;

        private NativeArray<Vector3> direction;
        private NativeArray<Vector3> velocity;
        private NativeArray<float> speeds;
        private TransformAccessArray transformAccessArray;

        private JobHandle moveJobHandle;

        private void Start()
        {
            allUnits = new List<Unit>();

            for (int i = 0; i < amount; i++)
            {
                Unit unit = Instantiate(prefab, new Vector3(Random.Range(-7f, 7f), Random.Range(-4f, 4f)), Quaternion.identity);
               
                unit.direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                unit.direction.Normalize();
                unit.speed = Random.Range(2f, 3f);

                allUnits.Add(unit);
            }

            direction = new NativeArray<Vector3>(allUnits.Count, Allocator.Persistent);
            directionsRead = new NativeArray<Vector3>(allUnits.Count, Allocator.Persistent);
            velocity = new NativeArray<Vector3>(allUnits.Count, Allocator.Persistent);
            positionsRead = new NativeArray<Vector3>(allUnits.Count, Allocator.Persistent);
            speeds = new NativeArray<float>(allUnits.Count, Allocator.Persistent);
            transformAccessArray = new TransformAccessArray(allUnits.Count);

            for (int i = 0; i < allUnits.Count; i++)
            {
                direction[i] = allUnits[i].direction;
                velocity[i] = allUnits[i].velocity;
                positionsRead[i] = allUnits[i].transform.position;
                speeds[i] = allUnits[i].speed;
                transformAccessArray.Add(allUnits[i].transform);
            }
        }

        private void OnDestroy()
        {
            direction.Dispose();
            directionsRead.Dispose();
            velocity.Dispose();
            positionsRead.Dispose();
            speeds.Dispose();
            transformAccessArray.Dispose();
        }

        private void Update()
        {
            MoveUnitJob moveUnitJob = new();

            moveUnitJob.Direction = direction;
            moveUnitJob.DirectionRead = directionsRead;
            moveUnitJob.Speeds = speeds;
            moveUnitJob.Velocity = velocity;
            moveUnitJob.PositionRead = positionsRead;
            moveUnitJob.DeltaTime = Time.deltaTime;
            moveUnitJob.Distance = distance;

            moveJobHandle = moveUnitJob.Schedule(transformAccessArray);
            moveJobHandle.Complete();

            for (int i = 0; i < allUnits.Count; i++)
            {
                directionsRead[i] = direction[i];
                positionsRead[i] = allUnits[i].transform.position;
            }
        }
    }

    [BurstCompile]
    public struct MoveUnitJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Direction;
        public NativeArray<Vector3> Velocity;
        [ReadOnly]
        public NativeArray<Vector3> DirectionRead;
        [ReadOnly]
        public NativeArray<Vector3> PositionRead;
        public NativeArray<float> Speeds;
        public float DeltaTime;
        public float Distance;

        public void Execute(int index, TransformAccess transform)
        {
            // Расчет направления
            Vector3 currentDirection = Direction[index];
            float nearCount = 0;
            for (int i = 0; i < DirectionRead.Length; i++)
            {
                if (transform.position == PositionRead[i]) continue;

                Vector3 dir = transform.position - PositionRead[i];

                if (dir.magnitude > Distance) continue;

                currentDirection += DirectionRead[i];
                nearCount++;
            }

            if (nearCount > 0)
            {
                Direction[index] += (currentDirection / nearCount);
            }

            if (transform.position.y > 5f) Direction[index] += Vector3.down;
            if (transform.position.y < -5f) Direction[index] += Vector3.up;

            if (transform.position.x > 7f) Direction[index] += Vector3.left;
            if (transform.position.x < -7f) Direction[index] += Vector3.right;

            Direction[index] = Direction[index].normalized;

            // Перемещение
            currentDirection.Normalize();
            transform.position += currentDirection * Speeds[index] * DeltaTime;
            transform.rotation = Quaternion.LookRotation(currentDirection);
        }
    }
}

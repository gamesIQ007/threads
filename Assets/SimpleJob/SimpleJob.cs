using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SimpleJob : MonoBehaviour
{
    private NativeArray<float> results;

    private ExampleJob job;
    private JobHandle jobHandle;

    private void Update()
    {
        results = new NativeArray<float>(1, Allocator.Persistent);

        job = new();

        job.DeltaTime = Time.deltaTime;
        job.Results = results;
        jobHandle = job.Schedule();
    }

    private void LateUpdate()
    {
        jobHandle.Complete();
        float value = job.Results[0];
        Debug.Log(value);
        results.Dispose();
    }
}

[BurstCompile]
public struct ExampleJob : IJob
{
    public NativeArray<float> Results;

    public double DeltaTime;

    public void Execute()
    {
        float value = 0;

        for (int i = 0; i < 500000; i++)
        {
            value += (float)System.Math.Sin((float)System.Math.Sqrt(DeltaTime));
        }

        Results[0] = value;
    }
}
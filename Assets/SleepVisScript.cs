using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepVisScript : MonoBehaviour 
{
    public TextAsset SourceData;
    public Material BoxMat;

    private void Start()
    {
        TimePoint[] sleepData = DataLoader.GetTimePoints(SourceData);
        List<Awakeness> awakenessData = GetAwakenessData(sleepData);
        DrawAwakeness(awakenessData);
        //DrawSleep(awakenessData);
    }

    private void DrawAwakeness(IEnumerable<Awakeness> awakenessData)
    {
        GameObject obj = new GameObject("Awakeness");
        foreach (Awakeness item in awakenessData)
        {
            GameObject cube = DrawBox(item.Start, item.End);
            cube.transform.parent = obj.transform;
        }
        obj.transform.localScale = new Vector3(100f, .1f, .1f);
    }

    private void DrawSleep(List<Awakeness> awakenessData)
    {
        GameObject obj = new GameObject("Sleeps");
        for (int i = 0; i < awakenessData.Count - 1; i++)
        {
            DateTime start = awakenessData[i].End;
            DateTime end = awakenessData[i + 1].Start;
            GameObject cube = DrawBox(start, end);
            cube.transform.parent = obj.transform;
        }
        obj.transform.localScale = new Vector3(.1f, .1f, .1f);
    }

    private GameObject DrawBox(DateTime start, DateTime end)
    {
        GameObject ret = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ret.GetComponent<MeshRenderer>().sharedMaterial = BoxMat;
        long span = end.Ticks - start.Ticks;
        ret.name = start.ToString() + " to " + end.ToString() + ", length: " + new TimeSpan(span).ToString();

        int day = start.DayOfYear;
        day = day / 7;
        start = start.AddDays(-day * 7);
        end = end.AddDays(-day * 7);
        long mid = (end.Ticks + start.Ticks) / 2;
        float floatSpan = span / 1000000000;
        float floatMid = (mid - 636336327000000000) / 1000000000;
        ret.transform.position = new Vector3(day * 1.1f, floatSpan, floatMid);
        ret.transform.localScale = new Vector3(1, floatSpan * 2, floatSpan);
        return ret;
    }

    private List<Awakeness> GetAwakenessData(TimePoint[] sleepData)
    {
        List<Awakeness> ret = new List<Awakeness>();
        TimePoint lastAwake = sleepData[0];
        for (int i = 1; i < sleepData.Length; i++)
        {
            TimePoint current = sleepData[i];
            if(current.State == ConciousnessState.Asleep)
            {
                Awakeness newAwakeness = new Awakeness() { Start = lastAwake.Time, End = current.Time };
                ret.Add(newAwakeness);
            }
            else
            {
                lastAwake = current;
            }
        }
        return ret;
    }

    private class Awakeness
    {
        public DateTime Start;
        public DateTime End;

        public override string ToString()
        {
            return (End - Start).ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

class DataLoader
{ 
    public static TimePoint[] GetTimePoints(TextAsset sourceText)
    {
        string[] data = sourceText.text.Split('\n');

        List<string> dayLines = null;
        List<TimePoint> times = new List<TimePoint>();

        foreach (string line in data)
        {
            string firstHalf = line.Split('-')[0];
            if (firstHalf.Trim() != "")
            {
                if (dayLines != null)
                {
                    List<TimePointBuilder> timePointBuilders = GetTimePointBuilders(dayLines).ToList();
                    IEnumerable<TimePoint> timePoints = GetTimePoints(timePointBuilders);
                    times.AddRange(timePoints);
                }
                dayLines = new List<string>();
            }

            dayLines.Add(line);
        }
        TimePoint[] ret = times.ToArray();
        return ret;
    }

    private static bool ContainsSleepOrAwake(string line)
    {
        bool containsAsleep = line.ToLower().Contains("sleep");
        bool containsAwake = line.ToLower().Contains("awake");
        return containsAsleep || containsAwake;
    }
    private static IEnumerable<TimePoint> GetTimePoints(List<TimePointBuilder> dayLines)
    {
        bool pmFound = false;
        for (int i = (dayLines.Count - 1); i >= 1; i--)
        {
            if(!dayLines[i].Am)
            {
                pmFound = true;
            }
            else
            {
                if(pmFound)
                {
                    dayLines[i].Time = dayLines[i].Time.AddHours(-12);
                }
            }
        }
        return dayLines.Select(item => item.ToTimepoint());
    }

    private static IEnumerable<TimePointBuilder> GetTimePointBuilders(List<string> dayLines)
    {
        string dateLine = dayLines[0];
        string[] dateText = dateLine.Split('-')[0].Trim().Split('/');
        int month = Convert.ToInt32(dateText[0]);
        int day = Convert.ToInt32(dateText[1]);
        int year = Convert.ToInt32(dateText[2]);

        string[] moneyLines = dayLines.Where(ContainsSleepOrAwake).ToArray();
        bool firstEntry = true;
        foreach (string line in moneyLines)
        {
            Debug.Log(line);
            ConciousnessState state = line.ToLower().Contains("sleep") ? ConciousnessState.Asleep : ConciousnessState.Awake;
            string timePortion = line.Split(']')[1].Trim();
            string endPortion = timePortion.Substring(timePortion.Length - 2, 2).ToLower();
            string numberPotion = timePortion.Substring(0, timePortion.Length - 2);
            string hoursString = numberPotion.Split(':')[0];
            int hours = Convert.ToInt32(hoursString) % 12;
            string minutesString = numberPotion.Split(':')[1];
            int minutes = Convert.ToInt32(minutesString);
            int actualDay = day;
            
            if ((endPortion == "pm" && hours != 12) || (endPortion == "am" && hours == 12))
            {
                hours += 12;
            }
            DateTime time = new DateTime(year, month, actualDay, hours, minutes, 0);

            if (!firstEntry && endPortion != "pm")
            {
                time = time.AddDays(1);
            }
            firstEntry = false;
            TimePointBuilder point = new TimePointBuilder(state, time, endPortion == "am");
            yield return point;
        }
    }
    
}

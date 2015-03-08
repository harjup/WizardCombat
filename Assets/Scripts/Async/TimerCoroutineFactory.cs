using UnityEngine;
using System.Collections;

public class TimerCoroutineFactory
{
    public IEnumerator CreateTimer(float seconds)
    {
        var timeRemaining = seconds;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
    }
}
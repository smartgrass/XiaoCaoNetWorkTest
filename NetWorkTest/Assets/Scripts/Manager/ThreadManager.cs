using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public  class ThreadManager:MonoSingleton<ThreadManager>
{
    private static bool NoUpdate = true;
    private static List<Action> UpdateQueue = new List<Action>();
    private static List<Action> UpdateRunQueue = new List<Action>();

    public static void ExecuteUpdate(Action action)
    {
        lock (UpdateQueue)
        {
            UpdateQueue.Add(action);
            NoUpdate = false;
        }
    }
    private void Update()
    {
        lock (UpdateQueue)
        {
            if (NoUpdate) return;
            UpdateRunQueue.AddRange(UpdateQueue);
            UpdateQueue.Clear();
            NoUpdate = true;
            for (var i = 0; i < UpdateRunQueue.Count; i++)
            {
                var action = UpdateRunQueue[i];
                if (action == null) continue;
                action();
            }
            UpdateRunQueue.Clear();
        }
    }
    public static void ExecuteDelay(Action action, float delayTime, bool timeScale = true)
    {
        //ExecuteCoroutine(DelayCoroutine(action, delayTime, timeScale));
    }

    private static IEnumerator DelayCoroutine(Action action, float delayTime, bool timeScale = true)
    {
        if (timeScale)
        {
            yield return new WaitForSeconds(delayTime);
        }
        else
        {
            yield return new WaitForSecondsRealtime(delayTime);
        }
        action();
    }



}


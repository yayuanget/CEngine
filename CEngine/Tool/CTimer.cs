using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CEngine
{
    public class CTimer
    {
        public static readonly CTimer instance = new CTimer();
        private static List<TimerEvent> eventList = new List<TimerEvent>();

        /// <summary>
        /// 添加一个无参的定时器事件，runTimes = -1 时为无数次
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="delay"></param>
        /// <param name="runTimes"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        public static void AddTimerEvent(string eventName, float delay, int runTimes, float interval, Callback callback)
        {
            if (callback == null)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> callback is null ");
                return;
            }

            if (delay < 0)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> delay can not less then 0 ");
                return;
            }

            if (interval < 0)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> interval can not less then 0 ");
                return;
            }

            if (runTimes <= 0 && runTimes != -1)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> runTimes can not less then 0 ");
                return;
            }

            TimerEvent timeEvent = new TimerEvent(eventName, delay, runTimes, interval, callback);

            //CDebug.Log("CTimer.AddTimeEvent eventName no parmas " + eventName);
            eventList.Add(timeEvent);
        }

        /// <summary>
        /// 添加一个有参的定时器事件，runTimes = -1 时为无数次
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="delay"></param>
        /// <param name="runTimes"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        public static void AddTimerEvent(string eventName, float delay, int runTimes, float interval, Callback<object[]> callback, params object[] parmas)
        {
            if (callback == null)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> callback is null ");
            }

            if (callback == null)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> callback is null ");
                return;
            }

            if (delay < 0)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> delay can not less then 0 ");
                return;
            }

            if (interval < 0)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> interval can not less then 0 ");
                return;
            }

            if (runTimes <= 0 && runTimes != -1)
            {
                CDebug.LogError("Error CTimer.AddTimeEvent -> runTimes can not less then 0 ");
                return;
            }

            TimerEvent timeEvent = new TimerEvent(eventName, delay, runTimes, interval, callback, parmas);

            //CDebug.Log("CTimer.AddTimeEvent eventName with parmas " + eventName);
            eventList.Add(timeEvent);
        }

        public bool IsHaveEvent(string eventName)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i].eventName.Equals(eventName))
                    return true;
            }
            return false;
        }

        public void Update()
        {
            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                if (eventList.Count == 0)
                    return;

                var timerEvent = eventList[i];
                timerEvent.Execute();
                if (timerEvent.isComplete)
                {
                    eventList.Remove(timerEvent);
                }
            }
        }

        public static void RemoveTimerEvent(string eventName)
        {
            //CDebug.Log("CTimer.RemoveTimeEvent eventName " + eventName);
            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                if (eventList[i].eventName.Equals(eventName))
                {
                    eventList[i].OnComplete();
                }
            }
        }

        public static void Dispose()
        {
            eventList.Clear();
        }
    }

    class TimerEvent
    {
        public string eventName;
        public Callback callback0;
        public Callback<object[]> callback1;
        public object[] parmas;
        public float startTime;
        public float dueTime; //引发初始定时器事件的时间。
        public int runTimes;
        public float interval;//间隔
        public bool isLoop { get { return runTimes == -1; } }

        public bool isComplete;

        public TimerEvent()
        {
            isComplete = false;
        }

        public TimerEvent(string eventName, float delay, int runTimes, float interval, Callback callback)
        {
            this.eventName = eventName;
            this.startTime = Time.time;
            this.dueTime = Time.time + delay;
            this.interval = interval;
            this.callback0 = callback;
            this.runTimes = runTimes;
            isComplete = false;
        }

        public TimerEvent(string eventName, float delay, int runTimes, float interval, Callback<object[]> callback, params object[] parmas)
        {
            this.eventName = eventName;
            this.startTime = Time.time;
            this.dueTime = Time.time + delay;
            this.interval = interval;
            this.callback1 = callback;
            this.parmas = parmas;
            this.runTimes = runTimes;
            isComplete = false;
        }

        public void Execute()
        {
            if (Time.time < dueTime)
                return;

            if (runTimes <= 0 && !isLoop)
                return;

            if (isComplete)
                return;

            if (!isLoop)
                runTimes--;

            if (callback0 != null)
                callback0.Invoke();
            if (callback1 != null)
                callback1.Invoke(parmas);

            if (runTimes <= 0 && !isLoop)
            {
                //CDebug.Log("OnComplete " + eventName);
                OnComplete();
                return;
            }

            dueTime = Time.time + interval;
        }

        public void OnComplete()
        {
            isComplete = true;
        }
    }
}


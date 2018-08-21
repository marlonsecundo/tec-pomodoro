using System;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Core;

namespace Tec.Pomodoro.Domain
{
    public enum TomatoStatus : int
    {
        Focus = 0,
        Free = 1,
        Waiting = 2,
        Stop
    }

    public delegate void TimerEventHandler(object state);

    public class Tomato
    {
        public event TimerEventHandler TimerTickEvent;
        public event TimerEventHandler TimerCompleteEvent;
        public event TimerEventHandler TimerStopEvent;

        public int MinFocus { get; }
        public int MinFree { get; }
        public int Replays { get; }
        public bool AbleInternet { get; }
        public int PomodoroCount { get; private set; }
        public TimeSpan TimerCount { get; set; }
        public TomatoStatus Status { get; private set; }
        public TomatoStatus PreviousStatus { get; private set; }

        private int step = 0;
        ThreadPoolTimer timer;

        public Tomato(int minFocus, int minFree, int replays, bool ableInternet)
        {
            MinFocus = minFocus;
            MinFree = minFree;
            Replays = replays;
            AbleInternet = ableInternet;
            PomodoroCount = 0;

            TimerCount = new TimeSpan();

            TimerCompleteEvent += TimerComplete;

        }

        public double GetTimeSecondsLeft()
        {
            int min = 0;

            switch (Status)
            {
                case TomatoStatus.Focus: min = MinFocus; break;
                case TomatoStatus.Free: min = MinFree; break;
                case TomatoStatus.Waiting: min = 30; break;
            }

            return TimeSpan.FromMinutes(min).TotalSeconds - TimerCount.TotalSeconds - 50;


        }

        private void Timer_Tick(object state)
        {
            TimerTickEvent(state);
            TimerCount += TimeSpan.FromSeconds(1);

            if (GetTimeSecondsLeft() <= 0)
            {
                TimerCount = new TimeSpan();
                if (step < Replays * 2)
                    TimerCompleteEvent(state);
                else
                    Stop();
            }
        }

        public void Start()
        {
            timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromSeconds(1));
            Status = TomatoStatus.Focus;
            PreviousStatus = Status;
            PomodoroCount = 1;
            step = 1;
        }


        public void Cancel()
        {
            if (timer != null)
            timer.Cancel();
        }



        public void Stop()
        {
            Status = TomatoStatus.Stop;
            timer.Cancel();
            TimerStopEvent(null);
        }

        private void TimerComplete(object state)
        {
            PreviousStatus = Status;

            if (Status == TomatoStatus.Focus)
            {
                if (PomodoroCount >= 4)
                {
                    Status = TomatoStatus.Waiting;
                    PomodoroCount = 0;
                }
                else
                    Status = TomatoStatus.Free;
            }
            else if (step < Replays * 2)
            {
                Status = TomatoStatus.Focus;
                PomodoroCount++;
            }

            step++;
        }

        public void InvokeTimerCompleteEvent()
        {
            TimerCompleteEvent(null);
        }
    }
}

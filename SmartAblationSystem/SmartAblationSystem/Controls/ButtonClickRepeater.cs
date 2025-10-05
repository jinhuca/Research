using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmartAblationSystem.Controls
{
    /// <summary>
    /// This class handles button click repeater.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class ButtonClickRepeater : Button
    {
        public event EventHandler Click;

        private int scrollIntervalMilliseconds = 200;
        private int intervalAccelerationFactor = 4;
        private int requiredTicksToAccelerate = 10;

        private bool enableScrollingAcceleration = true;

        private Timer timer2 = new Timer();

        int tickCounter = 0;

        /// <summary>
        /// This function handles button click repeater.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ButtonClickRepeater() : base()
        {
            timer2.Interval = scrollIntervalMilliseconds;
            timer2.Elapsed += Timer2_Elapsed;
            timer2.Stop();
        }
        /// <summary>
        /// This function gets/sets scrolling acceleration enable boolean value .
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EnableScrollingAcceleration
        {
            get
            {
                return enableScrollingAcceleration;
            }
            set
            {
                enableScrollingAcceleration = value;
            }
        }
        /// <summary>
        /// This function gets/sets scrolling interval value (milliseconds).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ScrollIntervalMilliseconds
        {
            get
            {
                return scrollIntervalMilliseconds;
            }
            set
            {
                scrollIntervalMilliseconds = value;
            }
        }
        /// <summary>
        /// This function gets/sets interval acceleration factor.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int IntervalAccelerationFactor
        {
            get
            {
                return intervalAccelerationFactor;
            }
            set
            {
                intervalAccelerationFactor = value;
            }
        }
        /// <summary>
        /// This function gets/sets required ticks to accelerate value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RequiredTicksToAccelerate
        {
            get
            {
                return requiredTicksToAccelerate;
            }
            set
            {
                requiredTicksToAccelerate = value;
            }
        }
        /// <summary>
        /// Executes the DoAction synchronously on the thread the Dispatcher is associated with.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>

        private void Timer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(DoAction);
        }
        /// Accelerates the interval after a certain amount of ticks.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void DoAction()
        {

            tickCounter++;

            try
            {
                //Accelerate the interval after a certain amount of tick.
                if (tickCounter > RequiredTicksToAccelerate &&
                    IntervalAccelerationFactor != 0 &&
                    timer2.Interval != (ScrollIntervalMilliseconds / IntervalAccelerationFactor))
                {
                    timer2.Interval = ScrollIntervalMilliseconds / IntervalAccelerationFactor;
                }

                OnClick(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        /// <summary>
        /// Accelerates the interval and starts timer when mouse down.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            OnClick(EventArgs.Empty);

            if (EnableScrollingAcceleration)
            {
                timer2.Interval = ScrollIntervalMilliseconds;
                timer2.Start();
            }
        }
        /// <summary>
        /// Stops timer and sets tick counter to 0 when mouse up.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (EnableScrollingAcceleration)
            {
                if (timer2.Enabled)
                {
                    timer2.Stop();
                }
                tickCounter = 0;
            }
        }
        /// <summary>
        /// Occurs when the OnClick event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        protected void OnClick(EventArgs e)
        {
            if (Click != null) Click(this, e);
        }
    }
}

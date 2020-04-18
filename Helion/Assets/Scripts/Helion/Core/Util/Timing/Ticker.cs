using System.Diagnostics;
using Helion.Core.Util.Logging;
using Debug = UnityEngine.Debug;

namespace Helion.Core.Util.Timing
{
    public class Ticker
    {
        private static readonly Log Log = LogManager.Instance();

        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly double stopwatchTicksPerTick;

        public float TickFraction => (float)(stopwatch.ElapsedTicks / stopwatchTicksPerTick);
        public Ticker(double millisPerTick)
        {
            Debug.Assert(millisPerTick > 0, "Cannot have a non-positive milliseconds per tick");

            if (!Stopwatch.IsHighResolution)
                Log.Error("Stopwatch timer is not high resolution, erroneous timings will likely result");

            stopwatchTicksPerTick = (long)(Stopwatch.Frequency * millisPerTick / 1000);
        }

        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Start();
        }

        public void Restart()
        {
            stopwatch.Restart();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Chronometer
{
    public class Chronometer : IChronometer
    {
        private Stopwatch stopWatch;
        private List<string> laps;

        public Chronometer()
        {
            stopWatch = new Stopwatch();
            laps = new List<string>();
        }

        public string GetTime => stopWatch.Elapsed.ToString($@"mm\:ss\.ffff");

        public List<string> Laps => laps;

        public string Lap()
        {
            string lap = GetTime;

            laps.Add(lap);

            return lap;
        }

        public void Reset()
        {
            stopWatch.Reset();
            laps = new List<string>();
        }

        public void Start()
        {
            stopWatch.Start();
        }

        public void Stop()
        {
            stopWatch.Stop();
        }
    }
}

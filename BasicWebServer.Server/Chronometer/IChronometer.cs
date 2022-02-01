using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Chronometer
{
    public interface IChronometer
    {
        string GetTime { get; }

        List<string> Laps { get; }

        void Start();

        void Stop();

        string Lap();

        void Reset();
    }
}

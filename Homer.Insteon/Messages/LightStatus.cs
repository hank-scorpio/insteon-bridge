using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    public class LightStatus : CommandResponse
    {
        public const byte MaxLevel      = byte.MaxValue;

        public DateTime Timestamp		{ get; } = DateTime.Now;

        public bool     IsFullOn        => Level == MaxLevel;
        public bool     IsOn            => Level > 0;
        public bool     IsOff           => !IsOn;
        public byte?    Level           => this[5];
        public byte?    AllLinkDelta    => this[4];
        public double   LevelPct        => Level.HasValue ? LevelToPct(Level.Value) : -1;
        public TimeSpan Age             => DateTime.Now - Timestamp;

        public override String ToString()
		{
            if (!Succeeded) return "N/A";
			if (IsOff) return "OFF";
            return $"{LevelPct:00%}";
        }
       
        public static byte PctToLevel(double pct)
            => (byte)(Math.Max(0, Math.Min(1, pct)) * MaxLevel);

        public static double LevelToPct(byte level)
            => level / (double)MaxLevel;
    }
}

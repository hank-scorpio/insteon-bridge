using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Homer.Insteon
{
    public class LightLevelCurve
    {

	    static LightLevelCurve Led { get; } 
		    = new LightLevelCurve("led", 0, 3, 6, 10, 15, 20, 25, 30, 36, 43, 50, 58, 67, 75, 85, 100);

	    static LightLevelCurve Incandescent { get; } 
		    = new LightLevelCurve("incandescent", 0, 8, 16, 24, 31, 38, 46, 52, 58, 64, 70, 76, 82, 88, 94, 100);
	
	    static LightLevelCurve Linear { get; } 
		    = new LightLevelCurve("linear", 0, 100);

	    static IEnumerable<LightLevelCurve> Defaults { get; } 
		    = new [] { Linear, Led, Incandescent };

	    public static LightLevelCurve GetByNameOrDefault(string name)
		    => Defaults.FirstOrDefault(x => x.Name == name) ?? Defaults.First();

	    private byte[] Levels { get; }

	    public string Name { get; }

	    public LightLevelCurve(string name, params byte[] levels)
	    {
		    this.Levels = levels;
		    Name = name;
	    }

	    public double this[double x, double max = 1] 
            => GetLevel(x, max);

	    double GetLevel(double x, double max = 1)
	    {
		    var step = (Levels.Length - 1) / max * x;
		    var i = (int)step;
		    double val = Levels[i];

		    if (i < Levels.Length - 1)
			    val += (step - i) * (Levels[i + 1] - val);

		    return val / 100d;
	    }

    }
}
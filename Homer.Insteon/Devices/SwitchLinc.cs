using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Homer.Insteon
{
    public class SwitchLinc : InsteonDevice
    {
        LightStatus status;
        public LightLevelCurve LevelCurve { get; }

        public LightStatus Status 
            => StatusCacheDuration > status?.Age ? status : (status = GetStatus().Result);

        public static TimeSpan DefaultStatusCacheDuration  { get; set; } 
            = TimeSpan.MaxValue;

        public TimeSpan StatusCacheDuration { get; set; } 
            = DefaultStatusCacheDuration;

        public SwitchLinc(InsteonController controller, InsteonId address, string name = null, string zone = null, string alias = null, LightLevelCurve levelCurve = null)
            : base(address, controller, name, zone, alias)
        {
            LevelCurve = levelCurve;
        }

        public override string ToString()
            => $"{base.ToString()} Level={status?.ToString() ?? "N/A"}";

        async Task<LightStatus> Run(Func<InsteonId, Task<LightStatus>> cmd)
            => status = await cmd(Address);

        public Task<LightStatus> SetLevelStep(int step, int maxStep)
            => SetLevel(LevelCurve[step,maxStep]);
        public Task<LightStatus> SetLevel(double level)
            => Run(a => Controller.SetLevelAsync(a, level));
        public Task<LightStatus> GetStatus() 
            => Run(Controller.GetStatus);
        public Task<LightStatus> SetFullOn() 
            => Run(Controller.SetFullOn);
        public Task<LightStatus> SetOff() 
            => Run(Controller.SetOff);
        public Task<LightStatus> Brighten() 
            => Run(Controller.Brighten);
        public Task<LightStatus> Dim() 
            => Run(Controller.Dim);
        public Task<LightStatus> RampUp(int durationMs = 0) 
            => Run(a => Controller.RampUp(a, durationMs));
        public Task<LightStatus> RampDown(int durationMs = 0) 
            => Run(a => Controller.RampDown(a, durationMs));
    }
}
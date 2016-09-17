using Homer.Insteon;
using Moar.Http;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsteonBridge
{
    [HttpGet("api")]
    public class LightsApi
    {
        House çonf;


        public LightsApi(House conf)
        {
            this.çonf = conf;
        }

        public object Serialize(SwitchLinc l)
        {
            return new
            {
                Id = l.Address.ToShortString(),
                l.Name,
                //Address = l.Address.ToString(),
                l.Zone,
                Level = (int) (l.LevelCurve[l.Status.LevelPct] / 2.55d),
                LevelRaw = (int)(l.Status.LevelPct * 100),
            };
        }

        IEnumerable<object> GetLights(Func<SwitchLinc, bool> predicate = null, Action<SwitchLinc> action = null)
        {
            var lights = çonf.Lights;

            if (predicate != null)
                lights = lights.Where(predicate);

            if (lights.Any() && action != null)
                lights.Each(action);

            return lights.Select(Serialize);
        }

        IEnumerable<object> GetLight(string id, Action<SwitchLinc> action)
            => GetLights(x => x.Address.ToShortString() == id || x.Alias.Equals(id), action);
        

        [HttpGet("lights")]
        public IEnumerable<object> GetAllLights(Func<SwitchLinc, bool> where = null)
            => GetLights();
        
        [HttpGet("lights/{id}")]
        public IEnumerable<object> GetLight(string id)
            => GetLight(id, l => l.GetStatus());
        
        [HttpGet("lights/{id}/brighten")]
        public IEnumerable<object> SetBrighten(string id)
            => GetLight(id, l => l.Brighten());
        
        [HttpGet("lights/{id}/dim")]
        public IEnumerable<object> SetDim(string id)
            => GetLight(id, l => l.Dim());
        
        [HttpGet("lights/{id}/~toggle")]
        public IEnumerable<object> Toggle(string id)
            => GetLight(id, l => { if (l.Status.IsOn) l.SetOff(); else l.SetFullOn(); });
        
        [HttpGet("lights/{id}/~on")]
        public IEnumerable<object> SetOn(string id)
            => GetLight(id, l => l.SetFullOn());
        
        [HttpGet("lights/{id}/~off")]
        public IEnumerable<object> SetOff(string id)
            => GetLight(id, l => l.SetOff());
        
        [HttpGet("lights/{id}/~{level}")]
        public IEnumerable<object> SetLevel(string id, int level)
            => GetLight(id, l => l.SetLevel(level / 100d));
        


    

    }
}

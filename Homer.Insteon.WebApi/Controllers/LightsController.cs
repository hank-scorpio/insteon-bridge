using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Homer.Insteon.WebApi.Controllers
{
    public class LightsController : ApiController
    {
        static IEnumerable<SwitchLinc> Lights 
            => Insteon.House.Lights;
      
        static SwitchLinc Light(string id)
            => Lights.FirstOrDefault(x => x.Address.ToShortString() == id || x.Alias == id);

        object ToJson(SwitchLinc l) 
            => l == null ? null : new
            {
                Id = l.Address.ToShortString(),
                l.Alias,
                l.Zone,
                l.Name,
                Level = (int)(l.Status.LevelPct * 100),
            };

        public IEnumerable<object> Get()
            => Lights.Select(ToJson);

        [Route("api/lights/{id}")]
        public object Get(string id)
            => ToJson(Light(id));

        [HttpGet, Route("api/lights/{id}/~{level}")]
        public async Task<object> SetLevel(string id, string level)
        {
            var light = Light(id);
            {
                switch (level.ToLower())
                {
                    case "s1":
                    case "s2":
                    case "s3":
                    case "s4":
                    case "s5":
                    case "s6":

                        await light.SetLevelStep(level[1] - '0', 7); break;
                    case "off":
                        await light.SetLevel(0); break;
                    case "on":
                        await light.SetLevel(1); break;
                    case "dim":
                        await light.Dim(); break;
                    case "brighten":
                        await light.Brighten(); break;
                    default:
                        int val = 0;
                        if (int.TryParse(level, out val) && val >= 0 && val <= 100)
                            await light.SetLevel(val / 100d);
                        break;
                }
            }
            await light.GetStatus();
            return ToJson(light);
        }

    }
}

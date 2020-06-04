using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfect
{
    public class PFConfigMapper : IPFConfigMapper
    {
        public List<PFModelConfigMapper> GetModelConfigMapper()
        {
            throw new NotImplementedException();
        }

        public PFNetworkConfig GetNetworkConfig()
        {
            return new PFNetworkConfig();
        }

        public PFPathConfig GetPathConfig()
        {
            return new PFPathConfig();
        }
    }
}

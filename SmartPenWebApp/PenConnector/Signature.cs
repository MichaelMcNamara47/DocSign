using Neosmartpen.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSignWebApp.PenConnector
{
    public class Signature : List<Stroke>
    {
        public new void Add(Stroke stroke)
        {

            base.Add(stroke);

        }

    }
}

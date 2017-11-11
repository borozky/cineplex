using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.api
{
    public class JSONResponse
    {
        public string status;
        public string message;
        public object data;

        public JSONResponse(string status = "success", string message = null, object data = null)
        {
            this.status = status;
            this.message = message;
            this.data = data;
        }
    }
}

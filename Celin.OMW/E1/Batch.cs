using System;
using System.Collections.Generic;
using System.Text;

namespace Celin.OMW
{
    public class Response
    {
        public AIS.Form<AIS.FormData<F0005.Row>> fs_0_DATABROWSE_F0005 { get; set; }
        public AIS.Form<AIS.FormData<F98220.Row>> fs_1_DATABROWSE_F98220 { get; set; }
    }
    public class Request : AIS.DatabrowserRequest
    {
        public Request()
        {
            batchDataRequest = true;
            dataRequests = new List<AIS.DatabrowserRequest>
            {
                new F0005.Request(),
                new F98220.Request()
            };
        }
    }
}

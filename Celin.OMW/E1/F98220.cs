using System;
using System.Collections.Generic;

namespace Celin.F98220
{
    public class Row
    {
        public DateTime F98220_OMWPD { get; set; }
        public string F98220_OMWPS { get; set; }
        public string F98220_OMWSV { get; set; }
        public string F98220_OMWDESC { get; set; }
        public DateTime F98220_OMWSD { get; set; }
        public string F98220_OMWPRJID { get; set; }
        public string F98220_OMWTYP { get; set; }
        public string F98220_USER { get; set; }
        public DateTime F98220_OMWCD { get; set; }
    }
    public class Response : AIS.FormResponse
    {
        public AIS.Form<AIS.FormData<Row>> fs_DATABROWSE_F98220 { get; set; }
    }
    public class Request : AIS.DatabrowserRequest
    {
        public Request()
        {
            outputType = "GRID_DATA";
            dataServiceType = "BROWSE";
            targetName = "F98220";
            targetType = "table";
            returnControlIDs = "OMWPRJID|OMWDESC|OMWPS|OMWTYP|OMWSV|OMCD|OMWSD|OMWPD|USER|OMWCD";
            maxPageSize = "1000";
            query = new AIS.Query
            {
                matchType = AIS.Query.MATCH_ALL,
                condition = new List<AIS.Condition>
                {
                    new AIS.Condition
                    {
                        controlId = "F98220.OMWPS",
                        @operator = AIS.Condition.BETWEEN,
                        value = new AIS.Value[]
                        {
                            new AIS.Value
                            {
                                content = "11",
                                specialValueId = AIS.Value.LITERAL
                            },
                            new AIS.Value
                            {
                                content = "37X",
                                specialValueId = AIS.Value.LITERAL
                            }
                        }
                    },
                    new AIS.Condition
                    {
                        controlId = "F98220.OMWPJS1",
                        @operator = AIS.Condition.STR_BLANK
                    },
                    new AIS.Condition
                    {
                        controlId = "F98220.OMWSD",
                        @operator = AIS.Condition.STR_NOT_BLANK
                    }
                }
            };
        }
    }
}

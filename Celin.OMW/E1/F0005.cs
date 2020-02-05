using System.Collections.Generic;

namespace Celin.F0005
{
    public class Row
    {
        public string F0005_KY { get; set; }
        public string F0005_RT { get; set; }
        public string F0005_DL01 { get; set; }
        public string F0005_SY { get; set; }
    }
    public class Response : AIS.FormResponse
    {
        public AIS.Form<AIS.FormData<Row>> fs_DATABROWSE_F0005 { get; set; }
    }
    public class Request : AIS.DatabrowserRequest
    {
        public Request()
        {
            outputType = "GRID_DATA";
            dataServiceType = "BROWSE";
            targetName = "F0005";
            targetType = "table";
            maxPageSize = "500";
            returnControlIDs = "SY|RT|KY|DL01";
            query = new AIS.Query
            {
                matchType = AIS.Query.MATCH_ALL,
                condition = new List<AIS.Condition>
                {
                    new AIS.Condition
                    {
                        controlId = "F0005.SY",
                        @operator = AIS.Condition.EQUAL,
                        value = new AIS.Value[]
                        {
                            new AIS.Value
                            {
                                content = "H92",
                                specialValueId = AIS.Value.LITERAL
                            }
                        }
                    },
                    new AIS.Condition
                    {
                        controlId = "F0005.RT",
                        @operator = AIS.Condition.LIST,
                        value = new AIS.Value[]
                        {
                            new AIS.Value
                            {
                                content = "PS",
                                specialValueId = AIS.Value.LITERAL
                            },
                            new AIS.Value
                            {
                                content = "AC",
                                specialValueId = AIS.Value.LITERAL
                            }
                        }
                    }
                }
            };
        }
    }
}

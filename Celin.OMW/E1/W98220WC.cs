using System;
using System.Collections.Generic;

namespace Celin.W98220WC
{
    public enum Fields
    {
        OMWDESC = 16,
        OMWPRJID = 14,
        OMWTYP = 20,
        OMWSV = 22,
        SYR = 24,
        SRCRLS = 77
    }
    public class FormData : AIS.FormData<AIS.Row>
    {
        public AIS.FormField<string> z_OMWPS_18 { get; set; }
        public AIS.FormField<string> z_OMWDESC_16 { get; set; }
        public AIS.FormField<string> z_OMWPCC5_59 { get; set; }
        public AIS.FormField<string> z_OMWPRJID_14 { get; set; }
        public AIS.FormField<string> z_OMWPCC4_57 { get; set; }
        public AIS.FormField<string> z_OMWPCC3_55 { get; set; }
        public AIS.FormField<string> z_STCM_98 { get; set; }
        public AIS.FormField<string> z_OMWPCC2_53 { get; set; }
        public AIS.FormField<string> z_WR07_95 { get; set; }
        public AIS.FormField<string> z_OMWPCC1_51 { get; set; }
        public AIS.FormField<string> z_OMWPCC10_69 { get; set; }
        public AIS.FormField<string> z_SYR_24 { get; set; }
        public AIS.FormField<string> z_OMWPCC9_67 { get; set; }
        public AIS.FormField<string> z_OMWSV_22 { get; set; }
        public AIS.FormField<string> z_OMWPCC8_65 { get; set; }
        public AIS.FormField<string> z_OMWTYP_20 { get; set; }
        public AIS.FormField<string> z_OMWPCC7_63 { get; set; }
        public AIS.FormField<string> z_OMWPCC6_61 { get; set; }
        public AIS.FormField<string> z_SRCRLS_77 { get; set; }
        public AIS.FormField<DateTime> z_OMWCD_75 { get; set; }
        public AIS.FormField<int> z_DOCO_48 { get; set; }
        public AIS.FormField<DateTime> z_OMWPD_46 { get; set; }
        public AIS.FormField<DateTime> z_OMWSD_44 { get; set; }
        public AIS.FormField<DateTime> z_OMWCMPD_82 { get; set; }
    }
    public class Response : AIS.FormResponse
    {
        public AIS.Form<FormData> fs_P98220W_W98220WC { get; set; }
    }
    public class Request : AIS.FormRequest
    {
        public void Ok() => formActions.Add(new AIS.FormAction
        {
            controlID = "11",
            command = AIS.FormAction.DoAction
        });
        public void Set(Fields field, string value)
        {
            var a = formActions ?? (formActions = new List<AIS.Action>());
            a.Add(new AIS.FormAction
            {
                controlID = Convert.ToInt32(field).ToString(),
                command = AIS.FormAction.SetControlValue,
                value = value
            });
        }
        public Request()
        {
            formName = "P98220W_W98220WC";
            version = "ZDJE0001";
        }
    }
}

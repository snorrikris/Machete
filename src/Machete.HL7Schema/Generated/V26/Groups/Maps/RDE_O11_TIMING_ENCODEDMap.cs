// This file was automatically generated and may be regenerated at any
// time. To ensure any changes are retained, modify the tool with any segment/component/group/field name
// or type changes.
namespace Machete.HL7Schema.V26.Maps
{
    using V26;

    /// <summary>
    /// RDE_O11_TIMING_ENCODED (GroupMap) - 
    /// </summary>
    public class RDE_O11_TIMING_ENCODEDMap :
        HL7V26LayoutMap<RDE_O11_TIMING_ENCODED>
    {
        public RDE_O11_TIMING_ENCODEDMap()
        {
            Segment(x => x.TQ1, 0, x => x.Required = true);
            Segment(x => x.TQ2, 1);
        }
    }
}
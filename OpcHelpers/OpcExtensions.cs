using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    internal static class OpcExtensions
    {
        /// <summary>
        /// Gets the datatype of an OPC tag
        /// </summary>
        /// <param name="tag">Tag to get datatype of</param>
        /// <returns>System Type</returns>
        //public static System.Type GetDataType(string tag)
        //{
        //    var nodesToRead = BuildReadValueIdCollection(tag, Attributes.Value);
        //    DataValueCollection results;
        //    DiagnosticInfoCollection diag;
        //    _session.Read(
        //        requestHeader: null,
        //        maxAge: 0,
        //        timestampsToReturn: TimestampsToReturn.Both,
        //        nodesToRead: nodesToRead,
        //        results: out results,
        //        diagnosticInfos: out diag);

        //    var type = results[0].WrappedValue.TypeInfo.BuiltInType;
        //    return System.Type.GetType("System." + type.ToString());

        //}
    }
}

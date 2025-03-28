using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace API.Infrastructure.Helpers
{
    public static class OracleHelpers
    {
        public static void AddWithValue(OracleCommand cmd, string name, object val, bool isOutput = false, bool isINOUT = false)
        {
            const int MAX_STRING_SIZE = 4000;
            var cmdAddMI = cmd.Parameters.GetType().GetMethod("Add", [typeof(string), typeof(object)]);
            cmdAddMI?.Invoke(cmd.Parameters, [name, val]);
            if (isOutput)
            {
                cmd.Parameters[name].Direction = ParameterDirection.Output;
            }
            if (isINOUT)
            {
                cmd.Parameters[name].Direction = ParameterDirection.InputOutput;
            }
            if (cmd.Parameters[name].DbType == DbType.String)
            {
                cmd.Parameters[name].Size = MAX_STRING_SIZE;
            }
        }

        public static dataType? ReadParameter<dataType>(OracleCommand cmd, string paramName)
        {
            //dataType obj = default;
            var data = cmd.Parameters[paramName].Value;
            if (data.Equals(DBNull.Value) | data.Equals(null))
            {
                return default;
            }
            return (dataType)data;
        }
    }
}

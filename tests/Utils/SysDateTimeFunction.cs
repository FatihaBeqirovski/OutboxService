using System;
using System.Data.SQLite;

namespace OutboxService.Tests.Utils
{
    [SQLiteFunction(Name = "SYSDATETIME", Arguments = 0, FuncType = FunctionType.Scalar)]
    public class SysDateTimeFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            var dateText = DateTime.Now.ToString("yyyyMMddhhmmss");
            return long.Parse(dateText);
        }
    }
}
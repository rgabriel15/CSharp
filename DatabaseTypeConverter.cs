using System;
using System.Data;

namespace RgSystems.Tools
{
    public static class DatabaseTypeConverter
    {
        public static DbType DotNetTypeToDbType(Type type)
        {
            switch (type.ToString())
            {
                case "Boolean":
                    return DbType.Boolean;
                case "Byte":
                    return DbType.Byte;
                case "Byte[]":
                    return DbType.Binary;
                case "Char[]":
                    return DbType.StringFixedLength;
                case "DateTime":
                    return DbType.DateTime;
                case "DateTimeOffset":
                    return DbType.DateTimeOffset;
                case "Decimal":
                    return DbType.Decimal;
                case "Double":
                    return DbType.Double;
                case "Guid":
                    return DbType.Guid;
                case "Int16":
                    return DbType.Int16;
                case "Int32":
                    return DbType.Int32;
                case "Int64":
                    return DbType.Int64;
                case "Object":
                    return DbType.Object;
                case "Short":
                    return DbType.Single;
                case "String":
                    return DbType.String;
                case "TimeSpan":
                    return DbType.Time;
                case "UInt16":
                    return DbType.UInt16;
                case "UInt32":
                    return DbType.UInt32;
                case "UInt64":
                    return DbType.UInt64;
                case "Xml":
                    return DbType.Xml;
                default:
                    throw new ArgumentException("type");
            }
        }

        public static Type DbTypeToDotNetType(DbType dbType)
        {
            switch (dbType.ToString())
            {
                case "AnsiString":
                case "String":
                    return typeof(string);
                case "Binary":
                    return typeof(byte[]);
                case "Boolean":
                    return typeof(bool);
                case "Byte":
                    return typeof(byte);
                case "Date ":
                case "DateTime":
                case "DateTime2":
                    return typeof(DateTime);
                case "DateTimeOffset":
                    return typeof(DateTimeOffset);
                case "Decimal":
                    return typeof(decimal);
                case "Double":
                    return typeof(double);
                case "Guid":
                    return typeof(Guid);
                case "Int16":
                    return typeof(short);
                case "Int32":
                    return typeof(int);
                case "Int64":
                    return typeof(long);
                case "Object":
                    return typeof(object);
                case "Single":
                    return typeof(short);
                case "StringFixedLength":
                    return typeof(char[]);
                case "Time":
                    return typeof(TimeSpan);
                case "UInt16":
                    return typeof(ushort);
                case "UInt32":
                    return typeof(uint);
                case "UInt64":
                    return typeof(ulong);
                default:
                    throw new ArgumentException("dbType");
            }
        }
    }
}

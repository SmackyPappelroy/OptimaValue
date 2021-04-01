using S7.Net;
using System;
using System.Linq;

namespace OptimaValue
{
    public static class PlcHelperClass
    {
        /// <summary>
        /// Konverterar S7-sträng till vanlig sträng
        /// <para>byte[0] = Maximal längd på strängen</para> 
        /// <para>byte[1] = Strängens total-längd</para> 
        /// <para>byte[3] - byte[byte1] = Strängen</para> 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string ConvertS7stringToString(byte[] byteArray)
        {
            byte currentLength = (byte)byteArray[1];

            var convText = new byte[currentLength];
            for (int i = 2; i < (int)currentLength + 2; i++)
            {
                convText[i - 2] = byteArray[i];
            }

            return System.Text.Encoding.ASCII.GetString(convText);
        }

        /// <summary>
        /// Konverterar Siemens DTL till <see cref="DateTime"/>
        /// </summary>
        /// <param name="dtl"></param>
        /// <returns></returns>
        public static DateTime DTLtoDateTime(DTL dtl)
        {
            var temp = dtl.Year.ToString();
            temp += "-" + (dtl.Month.ToString().Count() == 1 ? ("0" + dtl.Month.ToString()) : dtl.Month.ToString());
            temp += "-" + (dtl.Day.ToString().Count() == 1 ? ("0" + dtl.Day.ToString()) : dtl.Day.ToString());
            temp += " " + (dtl.Hour.ToString().Count() == 1 ? ("0" + dtl.Hour.ToString()) : dtl.Hour.ToString());
            temp += ":" + (dtl.Minute.ToString().Count() == 1 ? ("0" + dtl.Minute.ToString()) : dtl.Minute.ToString());
            temp += ":" + (dtl.Second.ToString().Count() == 1 ? ("0" + dtl.Second.ToString()) : dtl.Second.ToString());
            var temp2 = dtl.NanoSecond / 1000000;
            if (temp2.ToString().Length > 2)
                temp += "." + (temp2.ToString()).Substring(0, 3);
            else
                temp += "." + temp2.ToString();
            return Convert.ToDateTime(temp);
        }

        /// <summary>
        /// Konverterar <see cref="DateTime"/> till Siemens DTL
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DTL DateTimeToDTL(DateTime dt)
        {
            var dtl = new DTL()
            {
                Year = (short)dt.Year,
                Month = (byte)dt.Month,
                Day = (byte)dt.Day,
                Hour = (byte)dt.Hour,
                Minute = (byte)dt.Minute,
                Second = (byte)dt.Second,
                NanoSecond = dt.Millisecond * 1000000,
                WeekDay = (byte)(dt.DayOfWeek + 1),
            };
            return dtl;
        }

        public static string S7StringSwedish(this byte[] bytes)
        {
            {
                if (bytes.Length < 2)
                {
                    throw new PlcException(ErrorCode.ReadData, "Malformed S7 String / too short");
                }

                int size = bytes[0];
                int length = bytes[1];
                if (length > size)
                {
                    throw new PlcException(ErrorCode.ReadData, "Malformed S7 String / length larger than capacity");
                }

                try
                {
                    char[] chars = new char[length];
                    for (int i = 2; i < length + 2; i++)
                    {
                        chars[i - 2] = Convert.ToChar(bytes[i]);
                    }
                    return new string(chars);
                }
                catch (Exception e)
                {
                    throw new PlcException(ErrorCode.ReadData,
                        $"Failed to parse {VarType.S7String} from data. Following fields were read: size: '{size}', actual length: '{length}', total number of bytes (including header): '{bytes.Length}'.",
                        e);
                }

            }
        }

        public static string StringSwedish(this byte[] bytes)
        {
            {
                if (bytes.Length < 1)
                {
                    throw new PlcException(ErrorCode.ReadData, "Malformed S7 String / too short");
                }


                try
                {
                    char[] chars = new char[bytes.Count()];
                    for (int i = 0; i < bytes.Count() - 1; i++)
                    {
                        chars[i] = Convert.ToChar(bytes[i]);
                    }
                    return new string(chars);
                }
                catch (Exception e)
                {
                    throw new PlcException(ErrorCode.ReadData,
                        $"Failed to parse {VarType.S7String} from data. ",
                        e);
                }

            }
        }
    }
}

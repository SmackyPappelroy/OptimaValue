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
            try
            {
                DateTime dateTime = new DateTime(
                    year: dtl.Year,
                    month: dtl.Month,
                    day: dtl.Day,
                    hour: dtl.Hour,
                    minute: dtl.Minute,
                    second: dtl.Second,
                    millisecond: dtl.NanoSecond / 1000000 // Konverterar nanosekunder till millisekunder
                );

                return dateTime;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Hantera fall där DTL-värdena är utanför giltiga gränser
                throw new ArgumentException("Ogiltiga värden i DTL-objektet.", ex);
            }
        }


        /// <summary>
        /// Konverterar <see cref="DateTime"/> till Siemens DTL
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DTL DateTimeToDTL(DateTime dt)
        {
            // Justerar veckodag: DayOfWeek returnerar 0 för söndag, vi vill kanske justera för PLC-system
            byte weekDay = (byte)(dt.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)dt.DayOfWeek);

            // Skapar ett nytt DTL-objekt och tilldelar värden från DateTime
            var dtl = new DTL()
            {
                Year = (short)dt.Year, // DateTime.Year är alltid inom giltigt short-intervall
                Month = (byte)dt.Month, // Giltig månad från 1 till 12
                Day = (byte)dt.Day, // Giltigt datum från 1 till 31
                Hour = (byte)dt.Hour, // Giltig timme från 0 till 23
                Minute = (byte)dt.Minute, // Giltig minut från 0 till 59
                Second = (byte)dt.Second, // Giltig sekund från 0 till 59
                NanoSecond = dt.Millisecond * 1000000, // Konverterar millisekunder till nanosekunder
                WeekDay = weekDay // Veckodag, justerad enligt ovan
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
                    char[] chars = new char[bytes.Length];
                    for (int i = 0; i < bytes.Length; i++)
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

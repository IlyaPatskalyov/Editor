using System;

namespace Editor.Model
{
    internal static class OperationSerializer
    {
        public static string Serialize(Operation op)
        {
            return
                $"{(op.OperationType == OperationType.Insert ? '+' : '-')}{op.Char.Id.Value:x16}{op.Char.Next.Value:x16}{op.Char.Previous.Value:x16}{op.Char.Character}";
        }

        public static Operation Deserialize(string s)
        {
            return new Operation(s[0] == '+' ? OperationType.Insert : OperationType.Delete,
                                 new Char(new CharId(HexStringToUint(s.Substring(1, 16))),
                                          s.Substring(49),
                                          new CharId(HexStringToUint(s.Substring(33, 16))),
                                          new CharId(HexStringToUint(s.Substring(17, 16)))
                                 ));
        }

        private static uint HexStringToUint(string str)
        {
            uint value = 0;
            for (var i = 0; i < str.Length; i++)
            {
                value += HexCharToUint(str[i]) << ((str.Length - 1 - i) * 4);
            }
            return value;
        }

        private static uint HexCharToUint(char d)
        {
            if (d >= '0' && d <= '9')
                return (uint) (d - '0');
            d = char.ToLower(d);
            if (d >= 'a' && d <= 'f')
                return (uint) ((d - 'a') + 10);
            throw new Exception("HexCharToInt: input out of range for Hex value");
        }
    }
}
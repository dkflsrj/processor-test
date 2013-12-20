using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xmega32A4U_testBoard
{
    public static class Pifagor
    {
        public static class Hex
        {
            public static class toBin
            {
                public static string String(char HexDigit)
                {
                    switch (HexDigit)
                    {
                        case '0': return "0000";
                        case '1': return "0001";
                        case '2': return "0010";
                        case '3': return "0011";
                        case '4': return "0100";
                        case '5': return "0101";
                        case '6': return "0110";
                        case '7': return "0111";
                        case '8': return "1000";
                        case '9': return "1001";
                        case 'A': return "1010";
                        case 'B': return "1011";
                        case 'C': return "1100";
                        case 'D': return "1101";
                        case 'E': return "1110";
                        case 'F': return "1111";
                        default: return "?";
                    }
                }
                public static byte Byte(char HexDigit)
                {
                    switch (HexDigit)
                    {
                        case '0': return 0;
                        case '1': return 1;
                        case '2': return 2;
                        case '3': return 3;
                        case '4': return 4;
                        case '5': return 5;
                        case '6': return 6;
                        case '7': return 7;
                        case '8': return 8;
                        case '9': return 9;
                        case 'A': return 10;
                        case 'B': return 11;
                        case 'C': return 12;
                        case 'D': return 13;
                        case 'E': return 14;
                        case 'F': return 15;
                        default: return 255;
                    }
                }
            }
        }
    }
}

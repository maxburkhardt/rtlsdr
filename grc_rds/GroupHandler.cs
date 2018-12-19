using System;
using System.Linq;
using System.Text;

namespace GrcRdsServer
{
    static class GroupHandler
    {
        private static readonly StringBuilder _radioText = new StringBuilder("                                                                        ");
        private static readonly StringBuilder _programmeService = new StringBuilder("                                                                        ");

        public static void AnalyseFrames(ushort groupA, ushort groupB, ushort groupC, ushort groupD)
        {
            if ((groupB & 0xf800) == 0x2000) // 2a group radio text
            {
                int index = (groupB & 0xf) * 4; // text segment

                var sb = new StringBuilder();
                sb.Append((char)(groupC >> 8));
                sb.Append((char)(groupC & 0xff));
                sb.Append((char)(groupD >> 8));
                sb.Append((char)(groupD & 0xff));
                if (sb.ToString().Any(ch => (ch < ' ') || (ch > 0x7f)))
                {
                    return; // ignore garbage
                }

                _radioText.Remove(index, 4);
                _radioText.Insert(index, sb.ToString());

                //Console.WriteLine(theMessage.ToString());
            }

            if ((groupB & 0xf800) == 0x0000) // 0a group radio text
            {
                int index = (groupB & 0x3) * 2; // text segment

                var sb = new StringBuilder();
    
                sb.Append((char)(groupD >> 8));
                sb.Append((char)(groupD & 0xff));
                if (sb.ToString().Any(ch => (ch < ' ') || (ch > 0x7f)))
                {
                    return; // ignore garbage
                }

                _programmeService.Remove(index, 2);
                _programmeService.Insert(index, sb.ToString());

                Console.WriteLine(_programmeService.ToString());
            }
        }
    }
}

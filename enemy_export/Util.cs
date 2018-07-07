using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace enemy_export
{
    class Util
    {
        public static string readString(IntPtr address)
        {
            IntPtr name = new IntPtr(Marshal.ReadInt32(address));
            int len = 0;
            while (Marshal.ReadByte(name, len) != 0) len++;
            byte[] bytes = new byte[len];
            for (int i = 0; i < len; i++) bytes[i] = Marshal.ReadByte(name, i);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}

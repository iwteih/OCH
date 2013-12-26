using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OCHUtil
{
    public class EnumProcessesWrapper
    {
        [DllImport("Psapi.dll", SetLastError = true)]
        static extern bool EnumProcesses(
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds,
         UInt32 arraySizeBytes,
        [MarshalAs(UnmanagedType.U4)] out UInt32 bytesCopied
          );

        [Flags]
        enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("psapi.dll")]
        static extern uint GetModuleBaseNameA(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, uint nSize);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        public List<string> ShowAllProcessName()
        {
            List<string> list = new List<string>();

            UInt32 arraySize = 120;
            UInt32 arrayBytesSize = arraySize * sizeof(UInt32);
            UInt32[] processIds = new UInt32[arraySize];
            UInt32 bytesCopied;

            UInt32 numIdsCopied = arraySize;

            EnumProcesses(processIds, arrayBytesSize, out bytesCopied);

            for (UInt32 index = 0; index < numIdsCopied; index++)
            {
                IntPtr Handle;
                Handle = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VMRead, false, (int)processIds[index]);
                if ((int)Handle <= 4)
                    continue;

                StringBuilder baseName = new StringBuilder();

                GetModuleBaseNameA(Handle, (IntPtr)0, baseName, 100);

                list.Add(baseName.ToString());
            }

            return list;
        }
    }
}

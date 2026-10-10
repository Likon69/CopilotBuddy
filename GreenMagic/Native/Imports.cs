using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace GreenMagic.Native
{
	public static class Imports
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool QueryPerformanceFrequency(out long frequency);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr h);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostThreadMessage(int threadId, uint msg, uint wParam, uint lParam);

		[DllImport("user32")]
		public static extern bool EnumWindows(Imports.EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32", EntryPoint = "GetWindowText")]
		private static extern int GetWindowTextInternal(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		public static string GetWindowTitle(IntPtr hWnd, int nMaxCount)
		{
			StringBuilder sb = new StringBuilder(nMaxCount);
			int len = Imports.GetWindowTextInternal(hWnd, sb, nMaxCount);
			return len > 0 ? sb.ToString(0, len) : null;
		}

		public static string GetWindowTitle(IntPtr hWnd)
		{
			return Imports.GetWindowTitle(hWnd, 256);
		}

		[DllImport("user32", EntryPoint = "GetClassName")]
		private static extern int GetClassNameInternal(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		public static string GetClassName(IntPtr hWnd, int nMaxCount)
		{
			StringBuilder sb = new StringBuilder(nMaxCount);
			int len = Imports.GetClassNameInternal(hWnd, sb, nMaxCount);
			return len > 0 ? sb.ToString(0, len) : null;
		}

		public static string GetClassName(IntPtr hWnd)
		{
			return Imports.GetClassName(hWnd, 256);
		}

		[DllImport("user32")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

		[DllImport("user32")]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int dwProcessId);

		[DllImport("kernel32")]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32")]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32", EntryPoint = "GetModuleHandleW")]
		public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

		// Explicit GetModuleHandleW export used by Executor/ExecutorRand
		[DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr GetModuleHandleW(string lpModuleName);

		[DllImport("kernel32")]
		public static extern UIntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

		[DllImport("kernel32")]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll")]
		public static extern bool VirtualProtectEx(IntPtr hProcess, uint lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

		[DllImport("kernel32")]
		public static extern bool ReadProcessMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize, out int lpBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

		[DllImport("kernel32")]
		public static extern bool WriteProcessMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize, out IntPtr iBytesWritten);

		[DllImport("kernel32")]
		public static extern uint VirtualAllocEx(IntPtr hProcess, uint dwAddress, int nSize, uint dwAllocationType, uint dwProtect);

		[DllImport("kernel32")]
		public static extern bool VirtualFreeEx(IntPtr hProcess, uint dwAddress, int nSize, uint dwFreeType);

		[DllImport("kernel32.dll")]
		public static extern uint VirtualQueryEx(IntPtr hProcess, uint lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

		[DllImport("kernel32")]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr dwThreadId);

		[DllImport("kernel32")]
		public static extern uint WaitForSingleObject(IntPtr hObject, uint dwMilliseconds);

		[DllImport("kernel32")]
		public static extern bool GetExitCodeThread(IntPtr hThread, out UIntPtr lpExitCode);

		[DllImport("kernel32")]
		public static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

		[DllImport("kernel32")]
		public static extern uint SuspendThread(IntPtr hThread);

		[DllImport("kernel32")]
		public static extern uint ResumeThread(IntPtr hThread);

		[DllImport("kernel32")]
		public static extern uint TerminateThread(IntPtr hThread, uint dwExitCode);

		[DllImport("kernel32")]
		public static extern bool GetThreadContext(IntPtr hThread, ref Context lpContext);

		[DllImport("kernel32")]
		public static extern bool SetThreadContext(IntPtr hThread, ref Context lpContext);

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		// =================================================================
		// 64-bit target support
		//
		// The Win32 functions below take pointers, so their true signatures
		// are 64-bit capable — the uint-addressed overloads above simply
		// cannot express an address above 4 GB. These variants use
		// IntPtr/ulong so a 64-bit CopilotBuddy can attach to a native
		// 64-bit WoW process whose memory lives outside the low 4 GB.
		// =================================================================

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

		[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadProcessMemory64(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadProcessMemory64(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteProcessMemory64(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteProcessMemory64(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", EntryPoint = "VirtualAllocEx", SetLastError = true)]
		public static extern IntPtr VirtualAllocEx64(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", EntryPoint = "VirtualFreeEx", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualFreeEx64(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint dwFreeType);

		[DllImport("kernel32.dll", EntryPoint = "VirtualProtectEx", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualProtectEx64(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

		[DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx", SetLastError = true)]
		public static extern IntPtr VirtualQueryEx64(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, IntPtr dwLength);

		[DllImport("kernel32.dll", EntryPoint = "GetThreadContext", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetThreadContext64(IntPtr hThread, ref Context64 lpContext);

		[DllImport("kernel32.dll", EntryPoint = "SetThreadContext", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetThreadContext64(IntPtr hThread, ref Context64 lpContext);
	}
}

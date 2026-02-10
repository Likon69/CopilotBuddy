namespace GreenMagic
{
    /// <summary>
    /// Thrown when injected code causes a Windows exception (e.g., Access Violation)
    /// that was caught by the VEH handler. Contains the Windows NTSTATUS exception code.
    /// </summary>
    public class InjectionSEHException : Exception
    {
        /// <summary>
        /// Windows NTSTATUS exception code (e.g., 0xC0000005 for EXCEPTION_ACCESS_VIOLATION).
        /// </summary>
        public uint ExceptionCode { get; }

        public InjectionSEHException(uint exceptionCode)
            : base($"Injected code caused exception 0x{exceptionCode:X8}")
        {
            ExceptionCode = exceptionCode;
        }
    }

    /// <summary>
    /// Thrown when the injection synchronization times out, indicating the target
    /// process may have frozen or the hook was lost.
    /// </summary>
    public class InjectionDesyncException : Exception
    {
        public InjectionDesyncException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when VEH setup fails (TlsAlloc, AddVectoredExceptionHandler, TlsSetValue).
    /// </summary>
    public class InjectionException : Exception
    {
        public int StatusCode { get; }

        public InjectionException(int statusCode, string message)
            : base($"Injection error (status {statusCode}): {message}")
        {
            StatusCode = statusCode;
        }
    }
}

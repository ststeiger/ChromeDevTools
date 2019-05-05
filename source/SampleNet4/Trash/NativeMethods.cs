
namespace SampleNet4.Trash
{


    // https://github.com/mono/mono/blob/master/mcs/class/Mono.Posix/Mono.Unix.Native/Syscall.cs
    internal class NativeMethods
    {
        internal const string LIBC = "libc";


        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int getpid();


        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int setsid();

        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int getsid(int pid);



        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int setuid(uint uid);

        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern uint getuid();



        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int seteuid(uint euid);

        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern uint geteuid();



        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int setgid(uint gid);

        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern uint getgid();



        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int setegid(uint uid);

        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern uint getegid();


        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int getresuid(out uint ruid, out uint euid, out uint suid);

        // getresgid(2)
        //    int getresgid(gid_t *ruid, gid_t *euid, gid_t *suid);
        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int getresgid(out uint rgid, out uint egid, out uint sgid);

        // setresuid(2)
        //    int setresuid(uid_t ruid, uid_t euid, uid_t suid);
        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int setresuid(uint ruid, uint euid, uint suid);

        // setresgid(2)
        //    int setresgid(gid_t ruid, gid_t euid, gid_t suid);
        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int setresgid(uint rgid, uint egid, uint sgid);



        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
        public static extern int getgroups(int size, uint[] list);

        public static int getgroups(uint[] list)
        {
            return getgroups(list.Length, list);
        }


        [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true, EntryPoint = "kill")]
        private static extern int sys_kill(int pid, int sig);


        public enum Signum : int
        {
            SIGHUP = 1, // Hangup (POSIX).
            SIGINT = 2, // Interrupt (ANSI).
            SIGQUIT = 3, // Quit (POSIX).
            SIGILL = 4, // Illegal instruction (ANSI).
            SIGTRAP = 5, // Trace trap (POSIX).
            SIGABRT = 6, // Abort (ANSI).
            SIGIOT = 6, // IOT trap (4.2 BSD).
            SIGBUS = 7, // BUS error (4.2 BSD).
            SIGFPE = 8, // Floating-point exception (ANSI).
            SIGKILL = 9, // Kill, unblockable (POSIX).
            SIGUSR1 = 10, // User-defined signal 1 (POSIX).
            SIGSEGV = 11, // Segmentation violation (ANSI).
            SIGUSR2 = 12, // User-defined signal 2 (POSIX).
            SIGPIPE = 13, // Broken pipe (POSIX).
            SIGALRM = 14, // Alarm clock (POSIX).
            SIGTERM = 15, // Termination (ANSI).
            SIGSTKFLT = 16, // Stack fault.
            SIGCLD = SIGCHLD, // Same as SIGCHLD (System V).
            SIGCHLD = 17, // Child status has changed (POSIX).
            SIGCONT = 18, // Continue (POSIX).
            SIGSTOP = 19, // Stop, unblockable (POSIX).
            SIGTSTP = 20, // Keyboard stop (POSIX).
            SIGTTIN = 21, // Background read from tty (POSIX).
            SIGTTOU = 22, // Background write to tty (POSIX).
            SIGURG = 23, // Urgent condition on socket (4.2 BSD).
            SIGXCPU = 24, // CPU limit exceeded (4.2 BSD).
            SIGXFSZ = 25, // File size limit exceeded (4.2 BSD).
            SIGVTALRM = 26, // Virtual alarm clock (4.2 BSD).
            SIGPROF = 27, // Profiling alarm clock (4.2 BSD).
            SIGWINCH = 28, // Window size change (4.3 BSD, Sun).
            SIGPOLL = SIGIO, // Pollable event occurred (System V).
            SIGIO = 29, // I/O now possible (4.2 BSD).
            SIGPWR = 30, // Power failure restart (System V).
            SIGSYS = 31, // Bad system call.
            SIGUNUSED = 31
        }

        public static int kill(int pid, Signum sig)
        {
            int _sig = (int)sig; // NativeConvert.FromSignum(sig);
            return sys_kill(pid, _sig);
        }
    }

}

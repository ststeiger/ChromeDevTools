
#define DOTNET4


namespace System.Threading.Tasks
{


    public class Task2
    {


        public async static System.Threading.Tasks.Task Delay(int dueTime)
        {
#if DOTNET4
            await System.Threading.Tasks.TaskEx.Delay(dueTime);
#else
            await System.Threading.Tasks.Task.Delay(dueTime);
#endif
        }


        public static System.Threading.Tasks.Task Run(System.Action action
            , System.Threading.CancellationToken cancellationToken)
        {
#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(action, cancellationToken);
#else
            return System.Threading.Tasks.Task.Run(action, cancellationToken);
#endif
        }


        public static System.Threading.Tasks.Task Run(System.Action action)
        {
#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(action);
#else
            return System.Threading.Tasks.Task.Run(action);
#endif

        }


        public static System.Threading.Tasks.Task Run(System.Func<System.Threading.Tasks.Task> function)
        {
#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(function);
#else
            return System.Threading.Tasks.Task.Run(function);
#endif

        }


        public static System.Threading.Tasks.Task Run(System.Func<System.Threading.Tasks.Task> function
            , System.Threading.CancellationToken cancellationToken)
        {

#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(function, cancellationToken);
#else
            return System.Threading.Tasks.Task.Run(function, cancellationToken);
#endif


        }


        public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<System.Threading.Tasks.Task<TResult>> function)
        {


#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(function);
#else
            return System.Threading.Tasks.Task.Run(function);
#endif



        }


        public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<System.Threading.Tasks.Task<TResult>> function
            , System.Threading.CancellationToken cancellationToken)
        {


#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(function, cancellationToken);
#else
            return System.Threading.Tasks.Task.Run(function, cancellationToken);
#endif


        }


        public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<TResult> function)
        {

#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(function);
#else
            return System.Threading.Tasks.Task.Run(function);
#endif


        }


        public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<TResult> function
            , System.Threading.CancellationToken cancellationToken)
        {

#if DOTNET4
            return System.Threading.Tasks.TaskEx.Run(function, cancellationToken);
#else
            return System.Threading.Tasks.Task.Run(function, cancellationToken);
#endif

        }


    }


}

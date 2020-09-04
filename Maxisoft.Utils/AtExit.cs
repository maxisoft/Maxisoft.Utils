using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Maxisoft.Utils
{
    public enum RegisterType
    {
        /// <summary>
        /// Current registered action will be the first one executed 
        /// </summary>
        Prepend,

        /// <summary>
        /// Current registered action will be the last one executed 
        /// </summary>
        Append,
    }

    public interface IAtExit : IDisposable
    {
        IDisposable RegisterAtExit(Action<ExitReason> action, RegisterType registerType = RegisterType.Append);
        IDisposable RegisterAtExit(Action action, RegisterType registerType = RegisterType.Append);
    }

    internal class AtExit : IAtExit
    {
        public const int ExitTimeout = 3;
        public const int AtExitHandlerTimeout = 35;

        public static ILogger? Log;

        private readonly LinkedList<Action<ExitReason>> _handlers = new LinkedList<Action<ExitReason>>();

        public AtExit()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            //AppDomain.CurrentDomain.ProcessExit
        }

        public IDisposable RegisterAtExit(Action<ExitReason> action, RegisterType registerType = RegisterType.Append)
        {
            lock (_handlers)
            {
                var node = registerType == RegisterType.Append ? _handlers.AddLast(action) : _handlers.AddFirst(action);
                return new AtExitRegisterDisposable(this, node);
            }
        }

        public IDisposable RegisterAtExit(Action action, RegisterType registerType = RegisterType.Append)
            => RegisterAtExit(_ => action(), registerType);

        private class AtExitRegisterDisposable : IDisposable
        {
            private readonly AtExit _atExit;
            private readonly LinkedListNode<Action<ExitReason>> _node;

            public AtExitRegisterDisposable(AtExit atExit, LinkedListNode<Action<ExitReason>> node)
            {
                _atExit = atExit;
                _node = node;
            }

            public void Dispose()
            {
                lock (_atExit._handlers)
                {
                    _atExit._handlers.Remove(_node);
                }
            }
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            Log.LogWarning(e, "caught unhandled exception");
            //TODO logic
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler;
        }
    }

    public class ExitReason
    {
        public readonly int? ErrorCode;
        public readonly Exception Exception;
        public readonly string Message;
        public readonly string StackTrace;

        public ExitReason(int? errorCode, Exception exception, string message, string stackTrace)
        {
            ErrorCode = errorCode;
            Exception = exception;
            Message = message;
            StackTrace = stackTrace;
        }
    }
}
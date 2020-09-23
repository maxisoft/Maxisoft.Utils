using System;
using System.Threading;
using System.Threading.Tasks;

namespace Maxisoft.Utils.Empties
{
    [Obsolete("Use System.Threading.Tasks.ValueTask")]
    public class EmptyTask : Task
    {
        public EmptyTask() : base(new EmptyAction())
        {
        }

        public EmptyTask(CancellationToken cancellationToken) : base(new EmptyAction(), cancellationToken)
        {
        }

        public EmptyTask(CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(
            new EmptyAction(), cancellationToken, creationOptions)
        {
        }

        public EmptyTask(TaskCreationOptions creationOptions) : base(new EmptyAction(), creationOptions)
        {
        }

        public EmptyTask(object state) : base(new EmptyAction<object>(), state)
        {
        }

        public EmptyTask(object state, CancellationToken cancellationToken) : base(new EmptyAction<object>(), state,
            cancellationToken)
        {
        }

        public EmptyTask(object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(
            new EmptyAction<object>(), state, cancellationToken, creationOptions)
        {
        }

        public EmptyTask(object state, TaskCreationOptions creationOptions) : base(new EmptyAction<object>(), state,
            creationOptions)
        {
        }
    }
    
    [Obsolete("Use System.Threading.Tasks.ValueTask<T>")]
    public class EmptyTask<T> : Task<T>
    {
        public EmptyTask() : base(new EmptyFunc<T>())
        {
        }

        public EmptyTask(CancellationToken cancellationToken) : base(new EmptyFunc<T>(), cancellationToken)
        {
        }

        public EmptyTask(CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(
            new EmptyFunc<T>(), cancellationToken, creationOptions)
        {
        }

        public EmptyTask(TaskCreationOptions creationOptions) : base(new EmptyFunc<T>(), creationOptions)
        {
        }

        public EmptyTask(object state) : base(new EmptyFunc<object, T>(), state)
        {
        }

        public EmptyTask(object state, CancellationToken cancellationToken) : base(new EmptyFunc<object, T>(), state,
            cancellationToken)
        {
        }

        public EmptyTask(object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(
            new EmptyFunc<object, T>(), state, cancellationToken, creationOptions)
        {
        }

        public EmptyTask(object state, TaskCreationOptions creationOptions) : base(new EmptyFunc<object, T>(), state,
            creationOptions)
        {
        }
    }
}
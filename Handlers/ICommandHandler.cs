using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Handlers
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : notnull
    {
        Task<TResult> HandleAsync(TCommand command); // The method returns a Task that contains a TResult after processing the command.
    }

}
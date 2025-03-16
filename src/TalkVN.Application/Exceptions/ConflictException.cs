    using TalkVN.Application.Models;

namespace TalkVN.Application.Exceptions;
public class ConflictException : ApplicationException
{
    public ConflictException(string message, bool transactionRollback = true) : base(message)
    {
        Code = ApiResultErrorCodes.Conflict;
        TransactionRollback = transactionRollback;
    }
}

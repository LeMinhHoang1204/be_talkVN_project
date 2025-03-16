using TalkVN.Application.Models;

namespace TalkVN.Application.Exceptions;

[Serializable]
public class InvalidModelException : ApplicationException
{
    public InvalidModelException(string message, bool transactionRollback = true) : base(message)
    {
        Code = ApiResultErrorCodes.ModelValidation;
        TransactionRollback = transactionRollback;
    }
}

using TalkVN.Application.Models;

namespace TalkVN.Application.Exceptions
{
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message) : base(message)
        {
            Code = ApiResultErrorCodes.Forbidden;
        }
    }
}

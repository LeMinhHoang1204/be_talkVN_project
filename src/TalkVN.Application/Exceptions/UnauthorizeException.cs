namespace TalkVN.Application.Exceptions
{
    public class UnauthorizeException : ApplicationException
    {
        public UnauthorizeException(string message) : base(message)
        {
            Code = Models.ApiResultErrorCodes.Unauthorize;
        }
    }
}

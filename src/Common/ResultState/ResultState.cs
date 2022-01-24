using System;

namespace WorkforceManagementAPI.Common.ResultState
{
    public class ResultState
    {
        public ResultState(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }

        public ResultState(bool isSuccessful, string message, Exception exception)
        {
            IsSuccessful = isSuccessful;
            Message = message;
            Exception = exception;
        }

        public bool IsSuccessful { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}

using System;
using System.Runtime.CompilerServices;

namespace MvcMusicStore.ROP
{
    public class Result<TSuccess, TFailure>
    {
        public bool IsSuccessful;
        public TFailure Failure { get; set; }
        public TSuccess Success { get; set; }

        public static Result<TSuccess, TFailure> Succeded(TSuccess success)
        {
            if (success == null) throw new ArgumentException("Success can't be null");

            return new Result<TSuccess, TFailure>
                {
                    IsSuccessful = true,
                    Success = success
                }
                ;
        }

        public static Result<TSuccess, TFailure> Failed(TFailure fail)
        {
            if (fail == null) throw new ArgumentException("Success can't be null");

            return new Result<TSuccess, TFailure>
                {
                    IsSuccessful = false,
                    Failure = fail
                }
                ;
        }

        private Result()
        {
        }

    }
}
using System;

namespace MvcMusicStore.ROP
{
    public static class RopHelpers
    {
        public static Result<TSuccessNew, TFailure> Bind<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> prevResult,
            Func<TSuccess, Result<TSuccessNew, TFailure>> func)
        {
            if (!prevResult.IsSuccessful)
                return Result<TSuccessNew, TFailure>.Failed(prevResult.Failure);
            return func(prevResult.Success);
        }

        public static Result<TSuccess, TFailure> Tee<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> prevResult,
            Action<TSuccess> action)
        {
            if (prevResult.IsSuccessful)
            {
                action(prevResult.Success);
            }
            return prevResult;
        }

        public static Result<TSuccessNew, TFailure> Map<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> prevResult,
            Func<TSuccess, TSuccessNew> map)
        {
            return prevResult.IsSuccessful
                    ? Result<TSuccessNew, TFailure>.Succeded(map(prevResult.Success))
                    : Result<TSuccessNew, TFailure>.Failed(prevResult.Failure)
                ;
        }
    }
}
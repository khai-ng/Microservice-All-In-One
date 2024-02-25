﻿namespace Kernel.Result
{
    public interface IAppResult<T> : IAppResult
    {
        public T? Data { get; }
    }

    public interface IAppResult
    {
        public bool IsSuccess => Status == AppStatusCode.Ok;
        public AppStatusCode Status { get; }
        string? Message { get; }
        IEnumerable<ErrorDetail>? Errors { get; }
    }
}

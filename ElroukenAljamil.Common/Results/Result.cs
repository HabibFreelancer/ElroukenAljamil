using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Common.Results
{
    /// <summary>
    /// Pattern Result pour éviter les exceptions dans le flux métier.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        public string? ErrorCode { get; }

        protected Result(bool isSuccess, string? error, string? errorCode = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorCode = errorCode;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error, string? errorCode = null)
            => new(false, error, errorCode);

        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(string error, string? errorCode = null)
            => Result<T>.Failure(error, errorCode);
    }
    /// <summary>
    /// Result générique avec valeur de retour.
    /// </summary>
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? error, string? errorCode = null)
            : base(isSuccess, error, errorCode)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public new static Result<T> Failure(string error, string? errorCode = null)
            => new(false, default, error, errorCode);
    }

}

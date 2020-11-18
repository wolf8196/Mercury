using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Mercury.Models
{
    public class MercuryResult
    {
        public bool Success { get; set; }

        public int Status { get; set; }

        public IEnumerable<MercuryError> Errors { get; set; }

        public static MercuryResult OkResult()
        {
            return SuccessResult(StatusCodes.Status200OK);
        }

        public static MercuryResult AcceptedResult()
        {
            return SuccessResult(StatusCodes.Status202Accepted);
        }

        public static MercuryResult BadRequestResult(Result result)
        {
            return FailureResult(result, StatusCodes.Status400BadRequest);
        }

        public static MercuryResult InternalServerErrorResult()
        {
            return FailureResult(Result.Fail("Unhandled error occured."), StatusCodes.Status500InternalServerError);
        }

        private static MercuryResult SuccessResult(int status)
        {
            return new MercuryResult
            {
                Success = true,
                Status = status,
                Errors = new List<MercuryError>()
            };
        }

        private static MercuryResult FailureResult(Result result, int status)
        {
            return new MercuryResult
            {
                Success = false,
                Status = status,
                Errors = result.Errors.Select(x => MercuryError.Map(x)).ToList(),
            };
        }
    }
}
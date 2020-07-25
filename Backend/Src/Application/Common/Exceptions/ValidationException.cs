using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Failures { get; }
        
        public ValidationException() : base("One or more validation errors occured.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(List<ValidationFailure> failures) : base()
        {
            Failures = failures
                .Select(f => f.PropertyName)
                .Distinct()
                .ToDictionary(
                    p => p, 
                    p => failures.Where(f2 => f2.PropertyName == p).Select(f2 => f2.ErrorMessage).ToArray()
                );
        }
    }
}
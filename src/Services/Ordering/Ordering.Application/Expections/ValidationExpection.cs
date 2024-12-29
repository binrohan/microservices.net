using System;
using FluentValidation.Results;

namespace Ordering.Application.Expections;

public class ValidationExpection : ApplicationException
{
    public Dictionary<string, string[]> Errors { get; }
    public ValidationExpection()
        : base("One or more validation failures have occurred.")
    {
        Errors = [];
    }

    public ValidationExpection(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures.GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                         .ToDictionary(g => g.Key, g => g.ToArray());
    }
}

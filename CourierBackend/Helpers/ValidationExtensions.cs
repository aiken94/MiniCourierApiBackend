namespace CourierBackend.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation.Results;

    public static class ValidationExtensions
    {
        public static IDictionary<string, string[]> ToDictionary(
            this IEnumerable<ValidationFailure> failures)
        {
            return failures
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => char.ToLowerInvariant(g.Key[0]) + g.Key.Substring(1),
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
        }
    }
}
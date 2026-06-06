namespace CourierBackend.Helpers
{
    using Microsoft.Extensions.DependencyInjection;
    using FluentValidation;
    
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services)
        {
            var assembly = typeof(T).Assembly;

            foreach (var type in assembly.ExportedTypes)
            {
                if (!type.IsClass || type.IsAbstract)
                {
                    continue;
                }

                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                    {
                        continue;
                    }

                    if (interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                    {
                        services.AddScoped(interfaceType, type);
                    }
                }
            }

            return services;
        }
    }
}
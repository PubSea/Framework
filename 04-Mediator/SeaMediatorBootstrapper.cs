using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubSea.Mediator.Abstractions;
using PubSea.Mediator.Implementations;

namespace PubSea.Mediator;

public static class SeaMediatorBootstrapper
{
    public static IServiceCollection AddSeaMediator(this IServiceCollection services)
    {
        RegisterHandlers(typeof(ISeaRequestHandler<>), services);
        RegisterHandlers(typeof(ISeaRequestHandler<,>), services);

        services.TryAddScoped<ISeaMediator, SeaMediator>();

        return services;
    }

    private static void RegisterHandlers(Type handlerType, IServiceCollection services)
    {
        var handlers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
            .Where(p => p.IsClass && IsAssignableToGenericType(p, handlerType));

        foreach (var handler in handlers)
        {
            var args = handler.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType)
                ?.GetGenericArguments() ?? [];

            services.AddScoped(handlerType.MakeGenericType([.. args]), handler);
        }
    }

    static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        var baseType = givenType.BaseType!;
        if (baseType is null)
        {
            return false;
        }

        return IsAssignableToGenericType(baseType, genericType);
    }
}

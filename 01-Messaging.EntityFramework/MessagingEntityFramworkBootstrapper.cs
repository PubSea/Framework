using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubSea.Framework.DomainModel;
using PubSea.Messaging.EntityFramework.Configs;
using PubSea.Messaging.EntityFramework.EntityFramework;
using PubSea.Messaging.EntityFramework.HostedServices;
using PubSea.Messaging.EntityFramework.Services.Abstractions;
using PubSea.Messaging.EntityFramework.Services.Implementations;
using System.Reflection;
using System.Reflection.Emit;

namespace PubSea.Messaging.EntityFramework;

public static class MessagingEntityFramworkBootstrapper
{
    /// <summary>
    /// Adds messaging implementation.
    /// </summary>
    /// <typeparam name="DbContextType">The application dbcontext type</typeparam>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSeaMessaging<DbContextType>(this IServiceCollection services,
        Action<SeaMessagingConfig> config)
        where DbContextType : DbContext
    {
        SeaMessagingConfig newConfig = new()
        {
            Services = services,
        };
        config.Invoke(newConfig);

        services.AddSingleton(newConfig);

        services.AddSnowflakeService(cfg =>
        {
            cfg.GeneratorId = Random.Shared.Next(0, 60);
            cfg.Epoch = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            cfg.IdStructure = (43, 6, 14);
        });

        services.TryAddScoped<IOutboxService, OutboxService>();
        services.TryAddScoped<IInboxService, InboxService>();
        services.TryAddScoped<IConsumedFaultService, ConsumedFaultService>();
        services.TryAddScoped<ISeaPublisher, SeaPublisher>();
        services.TryAddScoped<IMainConsumer, MainConsumer>();
        services.TryAddScoped(typeof(ISeaMessagingDbContext), provider =>
        {
            var dbContext = provider.GetRequiredService<DbContextType>();
            return dbContext;
        });
        services.RegisterAllConsumers();
        services.RegisterAllMessageContracts();

        services.AddEfUnitOfWork<DbContextType>();


        if (newConfig.BrokerType == BrokerType.None)
        {
            services.TryAddSingleton<IBrokerService, DefaultBrokerService>();
        }

        services.AddHostedService<OutboxPollingPublisher>();
        services.AddHostedService<OutboxMessageTransporter>();
        services.AddHostedService<InboxCleaner>();

        services.AddDbContextPool<DbContextType>((provider, options) =>
        {
            options.UseSeaMessaging(provider);
        });

        return services;
    }

    private static void RegisterAllConsumers(this IServiceCollection services)
    {
        var consumers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.IsAbstract && !x.IsInterface && x.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISeaConsumer<>)))
            .ToList();

        var consumerMainType = typeof(ISeaConsumer<>);

        foreach (var consumer in consumers)
        {
            var typeArgs = consumer.GetInterfaces().First(i => i.Name == consumerMainType.Name)
                .GenericTypeArguments;
            var consumerType = consumerMainType.MakeGenericType(typeArgs);
            services.AddScoped(consumerType, consumer);
        }
    }

    private static void RegisterAllMessageContracts(this IServiceCollection services)
    {
        var consumers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.IsAbstract && !x.IsInterface && x.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISeaConsumer<>)))
            .ToList();

        foreach (var consumer in consumers)
        {
            var contract = consumer.GetInterface(typeof(ISeaConsumer<>).Name)
                ?.GenericTypeArguments.First();

            var classType = CreateContractClassType(contract!);
            MessageContracts.Types.Add(contract!.FullName!, classType);
        }
    }

    private static Type CreateContractClassType(Type interfaceType)
    {
        var name = "DynamicContractAssembly";
        var assembleyName = new AssemblyName(name);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assembleyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(assembleyName.Name ?? name);

        var typeBuilder = moduleBuilder.DefineType($"Real{interfaceType.Name}", TypeAttributes.Public);
        typeBuilder.AddInterfaceImplementation(interfaceType);

        foreach (var prop in interfaceType.GetProperties())
        {
            var fieldBuilder = typeBuilder.DefineField($"m_{prop.Name}",
                prop.PropertyType, FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(prop.Name, PropertyAttributes.HasDefault,
                prop.PropertyType, null);
            var getSetAttr = MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            var getBuilder = typeBuilder.DefineMethod($"get_{prop.Name}",
                getSetAttr, prop.PropertyType, Type.EmptyTypes);
            var getIL = getBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getBuilder);

            var setBuilder = typeBuilder.DefineMethod($"set_{prop.Name}",
                getSetAttr, null, [prop.PropertyType]);
            var setIL = setBuilder.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setBuilder);

            var interfaceGetMethod = prop.GetGetMethod();
            if (interfaceGetMethod is not null)
            {
                typeBuilder.DefineMethodOverride(getBuilder, interfaceGetMethod);
            }

            var interfaceSetMethod = prop.GetSetMethod();
            if (interfaceSetMethod is not null)
            {
                typeBuilder.DefineMethodOverride(setBuilder, interfaceSetMethod);
            }
        }

        return typeBuilder.CreateType();
    }
}

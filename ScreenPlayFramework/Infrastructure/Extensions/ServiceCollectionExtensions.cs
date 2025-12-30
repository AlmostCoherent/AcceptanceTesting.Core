using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a scoped instance the <see cref="IServiceCollection"/> for each type assignable to T from the
        /// specified assembly.
        /// </summary>
        /// <typeparam name="T">Used to find types in the assembly as assignable to that type
        /// to add to the <see cref="IServiceCollection"/>.</typeparam>
        /// <param name="services">An instance of <see cref="IServiceCollection"/></param>
        /// <param name="from"><see cref="Assembly"/> to find types in</param>
        /// <returns>The <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddScopedImplementingFromAssembly<T>(this IServiceCollection services, Assembly from)
        {
            from
                .GetTypesAssignableFrom<T>()
                .ToList()
                .ForEach(t => services.AddScoped(t));

            return services;
        }

        private static IEnumerable<Type> GetTypesAssignableFrom<T>(this Assembly assembly) => assembly.GetTypesAssignableFrom(typeof(T));

        private static IEnumerable<Type> GetTypesAssignableFrom(this Assembly assembly, Type compareType)
        {
            foreach (TypeInfo type in assembly.DefinedTypes)
            {
                if (compareType.IsAssignableFrom(type) && compareType != type)
                {
                    yield return type;
                }
            }
        }

    }
}

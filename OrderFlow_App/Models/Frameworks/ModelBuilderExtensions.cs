using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace OrderFlow_App.Models.Frameworks
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterAllEntities<TEntity>(
            this ModelBuilder modelBuilder,
            params Assembly[] assemblies)
            where TEntity : class
        {
            var types = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass &&
                            !c.IsAbstract &&
                            !c.IsGenericType &&
                            c.IsPublic &&
                            typeof(TEntity).IsAssignableFrom(c));

            foreach (var type in types)
            {
                modelBuilder.Entity(type);
            }
        }
    }
}

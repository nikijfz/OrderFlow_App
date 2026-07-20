using System.Reflection;
using OrderFlow_App.Models.Frameworks;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow_App.Models
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {
        }

        #region [- OnModelCreating() -]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region [- ApplyConfigurationsFromAssembly() -]
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            #endregion

            #region [- RegisterAllEntities() -]
            modelBuilder.RegisterAllEntities<IDbSetEntity>(typeof(IDbSetEntity).Assembly);
            #endregion

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

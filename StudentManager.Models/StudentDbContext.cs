using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace StudentManager.Models
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public bool IsRemoved { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
    
    public class StudentClass:BaseEntity
    {
        [Required]
        public string Name { get; set; }
        
    }

    public class Student : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
        [ForeignKey(nameof(Class))]
        public long ClassId { get; set; }
        public StudentClass Class { get; set; }
    }


    public class StudentDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(
                @"server=.;database=hello;uid=sa;pwd=sa");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           // modelBuilder.Entity<StudentClass>().HasQueryFilter(m => m.IsRemoved == false);
            
            
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            { 
                entityType.AddSoftDeleteQueryFilter();  
            }
        }
       
        public DbSet<StudentClass> StudentClasses { get; set; }

        public DbSet<Student> Students { get; set; }    
    }
    
    public static class SoftDeleteQueryExtension
    {
        public static void AddSoftDeleteQueryFilter(
            this IMutableEntityType entityData)
        {
            var methodToCall = typeof(SoftDeleteQueryExtension)
                .GetMethod(nameof(GetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(entityData.ClrType);
            if (methodToCall is { })
            {
                var filter = methodToCall.Invoke(null, new object[] { });
                entityData.SetQueryFilter((LambdaExpression)filter);
            }
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : BaseEntity
        {
            Expression<Func<TEntity, bool>> filter = x => !x.IsRemoved;
            return filter;
        }
    }
}
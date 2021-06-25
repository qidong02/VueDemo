using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using StudentManager.Models;

namespace StudentManager.Test
{
    interface IStudent
    {

    }
    class Student:IStudent
    {

    }
    
    public static class Extensions{
        public static void RegisterAllInterfaces(this ServiceCollection serviceCollection,params Assembly[] ass)
        {
            foreach (var assembly in ass)
            {
                foreach (var type in assembly.GetTypes().Where(m=>m.IsClass && m.GetInterfaces().Length>0 && m.Name != "BaseService") )
                {
                    serviceCollection.AddTransient(type.GetInterfaces().First(), type);
                }
            }
        }
    }
    class Program
    {
       
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            
            
            
            
           //serviceCollection.RegisterAllInterfaces(Assembly.Load(""),Assembly.Load(""));



            var provider = serviceCollection.BuildServiceProvider();

            var stu = provider.GetService<IStudent>();
            
            IStudent stu2 = new Student();


            Console.WriteLine("Hello World!");

            using (var db = new StudentDbContext())
            {
                // db.StudentClasses.First().IsRemoved = true;
                // db.SaveChanges();
                
                
                Console.WriteLine(db.StudentClasses.Count());
                
            }
        }
        
        
    }
}
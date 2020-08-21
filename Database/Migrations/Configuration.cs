namespace Database.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using GCRS.Base;
    using System.Data.Entity.SqlServer;
    using Microsoft.SqlServer.Types;

    internal sealed class Configuration : DbMigrationsConfiguration<DB>
    {
        public Configuration()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            SqlProviderServices.SqlServerTypesAssemblyName = typeof(SqlGeometry).Assembly.FullName;
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DB context)
        {
            DB db = context;
            db.Features.AddOrUpdate(
                p => p.Name,
                new Feature { Name="Aire acondicionado"},
                new Feature { Name="Gas licuado"},
                new Feature { Name = "Gas Natural" },
                new Feature { Name="Internet"}
            );

            db.RoomFeatures.AddOrUpdate(
                p=>p.Name,
                new RoomFeature { Name = "Closet" },
                new RoomFeature { Name="Aire Acondicionado"},
                new RoomFeature { Name = "Vista a exteriores" },
                new RoomFeature { Name = "Cama de Agua" },
                new RoomFeature { Name = "Cama Imperial" },
                new RoomFeature { Name = "TV" },
                new RoomFeature { Name = "Internet" }
            );

            db.RoomTypes.AddOrUpdate(
                p=>p.Name,
                new RoomType{ Name="Cuarto"},
                new RoomType { Name = "Cocina" },
                new RoomType { Name = "Comedor" },
                new RoomType { Name = "Baño" },
                new RoomType { Name = "Terraza" },
                new RoomType { Name = "Patio" }
            );

            Privileges admin = new Privileges { Name = "Admin" };
            Privileges realtor = new Privileges { Name = "Realtor" };
            Privileges client=new Privileges { Name = "Client" };
            Privileges jefe=new Privileges { Name = "Jefe" };

            db.Privileges.AddOrUpdate(
                p => p.Name,
                admin,realtor,client,jefe
                );

            db.Users.AddOrUpdate(
                p => p.Name,
                new User { Name = "admin", Pasword = "admin", Email = "admin",Privileges=admin },
                new User { Name = "realtor", Pasword = "realtor", Email = "realtor", Privileges = realtor },
                new User { Name = "jefe", Pasword = "jefe", Email = "jefe",Privileges=jefe }
                );

            db.Services.AddOrUpdate(
                p => p.Name,
                new Service { Name = "Tour", Price = 50 },
                new Service { Name = "Breakfast", Price = 10 }
                );

            //db.Houses.AddOrUpdate(
            //    new House
            //    {
            //        Description = "Testing House",
            //        OwnerID = 1,
            //        SquareMts = 3
            //    }
            //    );

            db.SaveChanges();
            
            //db.SalePublishes.AddOrUpdate(
            //    p=>p.House,
            //    new SalePublishing { House=new House { Place=new Place { Type=new PlaceType { Name="Casa"} ,Address="Maloja#4"} ,Features } }
            //    );
            //db.Rooms.AddOrUpdate(
            //);

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

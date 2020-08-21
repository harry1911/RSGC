namespace Database
{
    using GCRS.Base.APIDatabase;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class DB : DbContext,IDB
    {
        // Your context has been configured to use a 'Model1' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Database.Model1' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model1' 
        // connection string in the application configuration file.
        public DB()
            : base("name=DB")
        {
        }

        public System.Data.Entity.DbSet<GCRS.Base.User> Users { get; set; }

        //public System.Data.Entity.DbSet<GCRS.Base.GroupUser> GroupUsers { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.House> Houses { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.Feature> Features { get; set; }

        //public System.Data.Entity.DbSet<GCRS.Base.Place> Places { get; set; }

        //public System.Data.Entity.DbSet<GCRS.Base.PlaceType> PlaceTypes { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.Privileges> Privileges { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.Room> Rooms { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.RoomFeature> RoomFeatures { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.RoomType> RoomTypes { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.Service> Services { get; set; }

        public System.Data.Entity.DbSet<GCRS.Accounting.Concept> Concepts { get; set; }

        public System.Data.Entity.DbSet<GCRS.Accounting.DailySit> DailySit { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.RequestRentPublishing> RequestRentPublishings { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.RequestSalePublishing> RequestSalePublishings { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.BuyRequest> BuyRequests { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.RentRequest> RentRequests { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.SellRegistry> SellRegistries { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.RentRegistry> RentRegistries { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.Publishing> Publishes { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.RentPublishing> RentPublishes { get; set; }

        public System.Data.Entity.DbSet<GCRS.Base.SalePublishing> SalePublishes { get; set; }

        public IRepo<T> GetRepo<T>() where T : class
        {
            return new Repo<T>(this);
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}
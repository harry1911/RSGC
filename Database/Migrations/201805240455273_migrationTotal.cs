namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrationTotal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuyRequests",
                c => new
                    {
                        BuyRequestID = c.Int(nullable: false, identity: true),
                        RequestPrice = c.Double(nullable: false),
                        Negotiation = c.Boolean(nullable: false),
                        BuyState = c.Int(nullable: false),
                        LastModification = c.Int(nullable: false),
                        Client_UserID = c.Int(),
                        SellData_PublishingID = c.Int(),
                    })
                .PrimaryKey(t => t.BuyRequestID)
                .ForeignKey("dbo.Users", t => t.Client_UserID)
                .ForeignKey("dbo.Publishings", t => t.SellData_PublishingID)
                .Index(t => t.Client_UserID)
                .Index(t => t.SellData_PublishingID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(),
                        Phone = c.String(),
                        Pasword = c.String(nullable: false),
                        PrivilegesID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.Privileges", t => t.PrivilegesID, cascadeDelete: true)
                .Index(t => t.PrivilegesID);
            
            CreateTable(
                "dbo.Houses",
                c => new
                    {
                        HouseID = c.Int(nullable: false, identity: true),
                        OwnerID = c.Int(nullable: false),
                        Address = c.String(nullable: false),
                        Description = c.String(),
                        Location = c.Geometry(),
                        SquareMts = c.Single(nullable: false),
                        WaterFreq = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HouseID)
                .ForeignKey("dbo.Users", t => t.OwnerID, cascadeDelete: true)
                .Index(t => t.OwnerID);
            
            CreateTable(
                "dbo.Features",
                c => new
                    {
                        FeatureID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FeatureID);
            
            CreateTable(
                "dbo.RequestPublishings",
                c => new
                    {
                        RequestPublishingID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Price = c.Double(nullable: false),
                        CommissionPercent = c.Double(nullable: false),
                        LastChange = c.Int(nullable: false),
                        RequestStatus = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        House_HouseID = c.Int(),
                        Realtor_UserID = c.Int(),
                        User_UserID = c.Int(),
                        House_HouseID1 = c.Int(),
                    })
                .PrimaryKey(t => t.RequestPublishingID)
                .ForeignKey("dbo.Houses", t => t.House_HouseID)
                .ForeignKey("dbo.Users", t => t.Realtor_UserID)
                .ForeignKey("dbo.Users", t => t.User_UserID)
                .ForeignKey("dbo.Houses", t => t.House_HouseID1)
                .Index(t => t.House_HouseID)
                .Index(t => t.Realtor_UserID)
                .Index(t => t.User_UserID)
                .Index(t => t.House_HouseID1);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        ImageID = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        PublishingID = c.Int(),
                        RequestPublishingID = c.Int(),
                    })
                .PrimaryKey(t => t.ImageID)
                .ForeignKey("dbo.Publishings", t => t.PublishingID)
                .ForeignKey("dbo.RequestPublishings", t => t.RequestPublishingID)
                .Index(t => t.PublishingID)
                .Index(t => t.RequestPublishingID);
            
            CreateTable(
                "dbo.Publishings",
                c => new
                    {
                        PublishingID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Price = c.Double(nullable: false),
                        CommissionPercent = c.Double(nullable: false),
                        Showable = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Realtor_UserID = c.Int(),
                        User_UserID = c.Int(),
                        House_HouseID = c.Int(),
                        House_HouseID1 = c.Int(),
                    })
                .PrimaryKey(t => t.PublishingID)
                .ForeignKey("dbo.Users", t => t.Realtor_UserID)
                .ForeignKey("dbo.Users", t => t.User_UserID)
                .ForeignKey("dbo.Houses", t => t.House_HouseID)
                .ForeignKey("dbo.Houses", t => t.House_HouseID1)
                .Index(t => t.Realtor_UserID)
                .Index(t => t.User_UserID)
                .Index(t => t.House_HouseID)
                .Index(t => t.House_HouseID1);
            
            CreateTable(
                "dbo.RentRegistries",
                c => new
                    {
                        RegistryID = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        Finish = c.DateTime(nullable: false),
                        RentPrice = c.Double(nullable: false),
                        Client_UserID = c.Int(),
                        RentPublishing_PublishingID = c.Int(),
                    })
                .PrimaryKey(t => t.RegistryID)
                .ForeignKey("dbo.Users", t => t.Client_UserID)
                .ForeignKey("dbo.Publishings", t => t.RentPublishing_PublishingID)
                .Index(t => t.Client_UserID)
                .Index(t => t.RentPublishing_PublishingID);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        ServiceID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceID);
            
            CreateTable(
                "dbo.RentRequests",
                c => new
                    {
                        RentRequestID = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        Finish = c.DateTime(nullable: false),
                        RentPrice = c.Double(nullable: false),
                        RentState = c.Int(nullable: false),
                        LastChange = c.Int(nullable: false),
                        Client_UserID = c.Int(),
                        Publishing_PublishingID = c.Int(),
                    })
                .PrimaryKey(t => t.RentRequestID)
                .ForeignKey("dbo.Users", t => t.Client_UserID)
                .ForeignKey("dbo.Publishings", t => t.Publishing_PublishingID)
                .Index(t => t.Client_UserID)
                .Index(t => t.Publishing_PublishingID);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomID = c.Int(nullable: false, identity: true),
                        RoomTypeID = c.Int(nullable: false),
                        Description = c.String(),
                        SquareMts = c.Single(nullable: false),
                        HouseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoomID)
                .ForeignKey("dbo.Houses", t => t.HouseID, cascadeDelete: true)
                .ForeignKey("dbo.RoomTypes", t => t.RoomTypeID, cascadeDelete: true)
                .Index(t => t.RoomTypeID)
                .Index(t => t.HouseID);
            
            CreateTable(
                "dbo.RoomFeatures",
                c => new
                    {
                        RoomFeatureID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.RoomFeatureID);
            
            CreateTable(
                "dbo.RoomTypes",
                c => new
                    {
                        RoomTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.RoomTypeID);
            
            CreateTable(
                "dbo.Privileges",
                c => new
                    {
                        PrivilegesID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PrivilegesID);
            
            CreateTable(
                "dbo.Concepts",
                c => new
                    {
                        ConceptID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Income = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ConceptID);
            
            CreateTable(
                "dbo.DailySits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
                        TypeOfCompensation_ConceptID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Concepts", t => t.TypeOfCompensation_ConceptID)
                .Index(t => t.TypeOfCompensation_ConceptID);
            
            CreateTable(
                "dbo.SellRegistries",
                c => new
                    {
                        RegistryID = c.Int(nullable: false, identity: true),
                        Price = c.Double(nullable: false),
                        Client_UserID = c.Int(),
                        SellPublishing_PublishingID = c.Int(),
                    })
                .PrimaryKey(t => t.RegistryID)
                .ForeignKey("dbo.Users", t => t.Client_UserID)
                .ForeignKey("dbo.Publishings", t => t.SellPublishing_PublishingID)
                .Index(t => t.Client_UserID)
                .Index(t => t.SellPublishing_PublishingID);
            
            CreateTable(
                "dbo.FeatureHouses",
                c => new
                    {
                        Feature_FeatureID = c.Int(nullable: false),
                        House_HouseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Feature_FeatureID, t.House_HouseID })
                .ForeignKey("dbo.Features", t => t.Feature_FeatureID, cascadeDelete: true)
                .ForeignKey("dbo.Houses", t => t.House_HouseID, cascadeDelete: true)
                .Index(t => t.Feature_FeatureID)
                .Index(t => t.House_HouseID);
            
            CreateTable(
                "dbo.ServiceRentRegistries",
                c => new
                    {
                        Service_ServiceID = c.Int(nullable: false),
                        RentRegistry_RegistryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Service_ServiceID, t.RentRegistry_RegistryID })
                .ForeignKey("dbo.Services", t => t.Service_ServiceID, cascadeDelete: true)
                .ForeignKey("dbo.RentRegistries", t => t.RentRegistry_RegistryID, cascadeDelete: true)
                .Index(t => t.Service_ServiceID)
                .Index(t => t.RentRegistry_RegistryID);
            
            CreateTable(
                "dbo.RentRequestServices",
                c => new
                    {
                        RentRequest_RentRequestID = c.Int(nullable: false),
                        Service_ServiceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RentRequest_RentRequestID, t.Service_ServiceID })
                .ForeignKey("dbo.RentRequests", t => t.RentRequest_RentRequestID, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.Service_ServiceID, cascadeDelete: true)
                .Index(t => t.RentRequest_RentRequestID)
                .Index(t => t.Service_ServiceID);
            
            CreateTable(
                "dbo.RoomFeatureRooms",
                c => new
                    {
                        RoomFeature_RoomFeatureID = c.Int(nullable: false),
                        Room_RoomID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoomFeature_RoomFeatureID, t.Room_RoomID })
                .ForeignKey("dbo.RoomFeatures", t => t.RoomFeature_RoomFeatureID, cascadeDelete: true)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomID, cascadeDelete: true)
                .Index(t => t.RoomFeature_RoomFeatureID)
                .Index(t => t.Room_RoomID);
            
            CreateTable(
                "dbo.RoomRentPublishings",
                c => new
                    {
                        Room_RoomID = c.Int(nullable: false),
                        RentPublishing_PublishingID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Room_RoomID, t.RentPublishing_PublishingID })
                .ForeignKey("dbo.Rooms", t => t.Room_RoomID, cascadeDelete: true)
                .ForeignKey("dbo.Publishings", t => t.RentPublishing_PublishingID, cascadeDelete: true)
                .Index(t => t.Room_RoomID)
                .Index(t => t.RentPublishing_PublishingID);
            
            CreateTable(
                "dbo.RoomRequestRentPublishings",
                c => new
                    {
                        Room_RoomID = c.Int(nullable: false),
                        RequestRentPublishing_RequestPublishingID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Room_RoomID, t.RequestRentPublishing_RequestPublishingID })
                .ForeignKey("dbo.Rooms", t => t.Room_RoomID, cascadeDelete: true)
                .ForeignKey("dbo.RequestPublishings", t => t.RequestRentPublishing_RequestPublishingID, cascadeDelete: true)
                .Index(t => t.Room_RoomID)
                .Index(t => t.RequestRentPublishing_RequestPublishingID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SellRegistries", "SellPublishing_PublishingID", "dbo.Publishings");
            DropForeignKey("dbo.SellRegistries", "Client_UserID", "dbo.Users");
            DropForeignKey("dbo.DailySits", "TypeOfCompensation_ConceptID", "dbo.Concepts");
            DropForeignKey("dbo.BuyRequests", "SellData_PublishingID", "dbo.Publishings");
            DropForeignKey("dbo.BuyRequests", "Client_UserID", "dbo.Users");
            DropForeignKey("dbo.Users", "PrivilegesID", "dbo.Privileges");
            DropForeignKey("dbo.Images", "RequestPublishingID", "dbo.RequestPublishings");
            DropForeignKey("dbo.RequestPublishings", "House_HouseID1", "dbo.Houses");
            DropForeignKey("dbo.RequestPublishings", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.RequestPublishings", "Realtor_UserID", "dbo.Users");
            DropForeignKey("dbo.Images", "PublishingID", "dbo.Publishings");
            DropForeignKey("dbo.Publishings", "House_HouseID1", "dbo.Houses");
            DropForeignKey("dbo.Rooms", "RoomTypeID", "dbo.RoomTypes");
            DropForeignKey("dbo.RoomRequestRentPublishings", "RequestRentPublishing_RequestPublishingID", "dbo.RequestPublishings");
            DropForeignKey("dbo.RoomRequestRentPublishings", "Room_RoomID", "dbo.Rooms");
            DropForeignKey("dbo.RoomRentPublishings", "RentPublishing_PublishingID", "dbo.Publishings");
            DropForeignKey("dbo.RoomRentPublishings", "Room_RoomID", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "HouseID", "dbo.Houses");
            DropForeignKey("dbo.RoomFeatureRooms", "Room_RoomID", "dbo.Rooms");
            DropForeignKey("dbo.RoomFeatureRooms", "RoomFeature_RoomFeatureID", "dbo.RoomFeatures");
            DropForeignKey("dbo.RentRequestServices", "Service_ServiceID", "dbo.Services");
            DropForeignKey("dbo.RentRequestServices", "RentRequest_RentRequestID", "dbo.RentRequests");
            DropForeignKey("dbo.RentRequests", "Publishing_PublishingID", "dbo.Publishings");
            DropForeignKey("dbo.RentRequests", "Client_UserID", "dbo.Users");
            DropForeignKey("dbo.ServiceRentRegistries", "RentRegistry_RegistryID", "dbo.RentRegistries");
            DropForeignKey("dbo.ServiceRentRegistries", "Service_ServiceID", "dbo.Services");
            DropForeignKey("dbo.RentRegistries", "RentPublishing_PublishingID", "dbo.Publishings");
            DropForeignKey("dbo.RentRegistries", "Client_UserID", "dbo.Users");
            DropForeignKey("dbo.Publishings", "House_HouseID", "dbo.Houses");
            DropForeignKey("dbo.Publishings", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.Publishings", "Realtor_UserID", "dbo.Users");
            DropForeignKey("dbo.RequestPublishings", "House_HouseID", "dbo.Houses");
            DropForeignKey("dbo.Houses", "OwnerID", "dbo.Users");
            DropForeignKey("dbo.FeatureHouses", "House_HouseID", "dbo.Houses");
            DropForeignKey("dbo.FeatureHouses", "Feature_FeatureID", "dbo.Features");
            DropIndex("dbo.RoomRequestRentPublishings", new[] { "RequestRentPublishing_RequestPublishingID" });
            DropIndex("dbo.RoomRequestRentPublishings", new[] { "Room_RoomID" });
            DropIndex("dbo.RoomRentPublishings", new[] { "RentPublishing_PublishingID" });
            DropIndex("dbo.RoomRentPublishings", new[] { "Room_RoomID" });
            DropIndex("dbo.RoomFeatureRooms", new[] { "Room_RoomID" });
            DropIndex("dbo.RoomFeatureRooms", new[] { "RoomFeature_RoomFeatureID" });
            DropIndex("dbo.RentRequestServices", new[] { "Service_ServiceID" });
            DropIndex("dbo.RentRequestServices", new[] { "RentRequest_RentRequestID" });
            DropIndex("dbo.ServiceRentRegistries", new[] { "RentRegistry_RegistryID" });
            DropIndex("dbo.ServiceRentRegistries", new[] { "Service_ServiceID" });
            DropIndex("dbo.FeatureHouses", new[] { "House_HouseID" });
            DropIndex("dbo.FeatureHouses", new[] { "Feature_FeatureID" });
            DropIndex("dbo.SellRegistries", new[] { "SellPublishing_PublishingID" });
            DropIndex("dbo.SellRegistries", new[] { "Client_UserID" });
            DropIndex("dbo.DailySits", new[] { "TypeOfCompensation_ConceptID" });
            DropIndex("dbo.Rooms", new[] { "HouseID" });
            DropIndex("dbo.Rooms", new[] { "RoomTypeID" });
            DropIndex("dbo.RentRequests", new[] { "Publishing_PublishingID" });
            DropIndex("dbo.RentRequests", new[] { "Client_UserID" });
            DropIndex("dbo.RentRegistries", new[] { "RentPublishing_PublishingID" });
            DropIndex("dbo.RentRegistries", new[] { "Client_UserID" });
            DropIndex("dbo.Publishings", new[] { "House_HouseID1" });
            DropIndex("dbo.Publishings", new[] { "House_HouseID" });
            DropIndex("dbo.Publishings", new[] { "User_UserID" });
            DropIndex("dbo.Publishings", new[] { "Realtor_UserID" });
            DropIndex("dbo.Images", new[] { "RequestPublishingID" });
            DropIndex("dbo.Images", new[] { "PublishingID" });
            DropIndex("dbo.RequestPublishings", new[] { "House_HouseID1" });
            DropIndex("dbo.RequestPublishings", new[] { "User_UserID" });
            DropIndex("dbo.RequestPublishings", new[] { "Realtor_UserID" });
            DropIndex("dbo.RequestPublishings", new[] { "House_HouseID" });
            DropIndex("dbo.Houses", new[] { "OwnerID" });
            DropIndex("dbo.Users", new[] { "PrivilegesID" });
            DropIndex("dbo.BuyRequests", new[] { "SellData_PublishingID" });
            DropIndex("dbo.BuyRequests", new[] { "Client_UserID" });
            DropTable("dbo.RoomRequestRentPublishings");
            DropTable("dbo.RoomRentPublishings");
            DropTable("dbo.RoomFeatureRooms");
            DropTable("dbo.RentRequestServices");
            DropTable("dbo.ServiceRentRegistries");
            DropTable("dbo.FeatureHouses");
            DropTable("dbo.SellRegistries");
            DropTable("dbo.DailySits");
            DropTable("dbo.Concepts");
            DropTable("dbo.Privileges");
            DropTable("dbo.RoomTypes");
            DropTable("dbo.RoomFeatures");
            DropTable("dbo.Rooms");
            DropTable("dbo.RentRequests");
            DropTable("dbo.Services");
            DropTable("dbo.RentRegistries");
            DropTable("dbo.Publishings");
            DropTable("dbo.Images");
            DropTable("dbo.RequestPublishings");
            DropTable("dbo.Features");
            DropTable("dbo.Houses");
            DropTable("dbo.Users");
            DropTable("dbo.BuyRequests");
        }
    }
}

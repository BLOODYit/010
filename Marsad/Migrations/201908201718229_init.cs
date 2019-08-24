namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bundles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 255),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Indicators",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 1024),
                        Name = c.String(nullable: false, maxLength: 1024),
                        ElementCount = c.Int(nullable: false),
                        MeasureUnit = c.String(),
                        HasParent = c.Boolean(nullable: false),
                        IndicatorID = c.Int(),
                        IndicatorTypeID = c.Int(nullable: false),
                        BundleID = c.Int(nullable: false),
                        Description = c.String(),
                        Correlation = c.String(),
                        GeoArea = c.String(),
                        References = c.String(),
                        CalculationMethod = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Bundles", t => t.BundleID, cascadeDelete: true)
                .ForeignKey("dbo.Indicators", t => t.IndicatorID)
                .ForeignKey("dbo.IndicatorTypes", t => t.IndicatorTypeID, cascadeDelete: true)
                .Index(t => t.IndicatorID)
                .Index(t => t.IndicatorTypeID)
                .Index(t => t.BundleID);
            
            CreateTable(
                "dbo.Cases",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                        Year = c.Int(nullable: false),
                        PeriodID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Periods", t => t.PeriodID)
                .Index(t => t.PeriodID);
            
            CreateTable(
                "dbo.CaseYears",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CaseID = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Cases", t => t.CaseID, cascadeDelete: true)
                .Index(t => t.CaseID);
            
            CreateTable(
                "dbo.CaseYearIndicators",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CaseYearID = c.Int(nullable: false),
                        IndicatorID = c.Int(nullable: false),
                        IndicatorType = c.Short(nullable: false),
                        Strategy = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CaseYears", t => t.CaseYearID, cascadeDelete: true)
                .ForeignKey("dbo.Indicators", t => t.IndicatorID, cascadeDelete: true)
                .Index(t => t.CaseYearID)
                .Index(t => t.IndicatorID);
            
            CreateTable(
                "dbo.Entities",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DataSources",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        PublishDate = c.DateTime(nullable: false),
                        IsHijri = c.Boolean(nullable: false),
                        DataSourceTypeID = c.Int(nullable: false),
                        PublishNumber = c.String(maxLength: 255),
                        PublisherName = c.String(),
                        AuthorName = c.String(),
                        NoPeriod = c.Boolean(nullable: false),
                        PeriodID = c.Int(),
                        NoEntity = c.Boolean(nullable: false),
                        EntityID = c.Int(),
                        IsPart = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DataSourceTypes", t => t.DataSourceTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Entities", t => t.EntityID)
                .ForeignKey("dbo.Periods", t => t.PeriodID)
                .Index(t => t.DataSourceTypeID)
                .Index(t => t.PeriodID)
                .Index(t => t.EntityID);
            
            CreateTable(
                "dbo.DataSourceGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DataSourceTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Elements",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 255),
                        Name = c.String(nullable: false, maxLength: 255),
                        MeasureUnit = c.String(maxLength: 255),
                        DataSourceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DataSources", t => t.DataSourceID, cascadeDelete: true)
                .Index(t => t.DataSourceID);
            
            CreateTable(
                "dbo.Equations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IndicatorID = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        EquationText = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Indicators", t => t.IndicatorID, cascadeDelete: true)
                .Index(t => t.IndicatorID);
            
            CreateTable(
                "dbo.Periods",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.IndicatorGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 255),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.IndicatorTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GeoAreaBundles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GeoAreas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        GeoAreaID = c.Int(),
                        Type = c.String(nullable: false),
                        GeoAreaBundle_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.GeoAreas", t => t.GeoAreaID)
                .ForeignKey("dbo.GeoAreaBundles", t => t.GeoAreaBundle_ID)
                .Index(t => t.GeoAreaID)
                .Index(t => t.GeoAreaBundle_ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserGroupID = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserGroups", t => t.UserGroupID)
                .Index(t => t.UserGroupID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.MyClaims",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.EntityCases",
                c => new
                    {
                        Entity_ID = c.Int(nullable: false),
                        Case_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Entity_ID, t.Case_ID })
                .ForeignKey("dbo.Entities", t => t.Entity_ID, cascadeDelete: true)
                .ForeignKey("dbo.Cases", t => t.Case_ID, cascadeDelete: true)
                .Index(t => t.Entity_ID)
                .Index(t => t.Case_ID);
            
            CreateTable(
                "dbo.DataSourceGroupDataSources",
                c => new
                    {
                        DataSourceGroup_ID = c.Int(nullable: false),
                        DataSource_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DataSourceGroup_ID, t.DataSource_ID })
                .ForeignKey("dbo.DataSourceGroups", t => t.DataSourceGroup_ID, cascadeDelete: true)
                .ForeignKey("dbo.DataSources", t => t.DataSource_ID, cascadeDelete: true)
                .Index(t => t.DataSourceGroup_ID)
                .Index(t => t.DataSource_ID);
            
            CreateTable(
                "dbo.EquationElements",
                c => new
                    {
                        Equation_ID = c.Int(nullable: false),
                        Element_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Equation_ID, t.Element_ID })
                .ForeignKey("dbo.Equations", t => t.Equation_ID, cascadeDelete: true)
                .ForeignKey("dbo.Elements", t => t.Element_ID, cascadeDelete: true)
                .Index(t => t.Equation_ID)
                .Index(t => t.Element_ID);
            
            CreateTable(
                "dbo.CaseIndicators",
                c => new
                    {
                        Case_ID = c.Int(nullable: false),
                        Indicator_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Case_ID, t.Indicator_ID })
                .ForeignKey("dbo.Cases", t => t.Case_ID, cascadeDelete: true)
                .ForeignKey("dbo.Indicators", t => t.Indicator_ID, cascadeDelete: true)
                .Index(t => t.Case_ID)
                .Index(t => t.Indicator_ID);
            
            CreateTable(
                "dbo.IndicatorGroupIndicators",
                c => new
                    {
                        IndicatorGroup_ID = c.Int(nullable: false),
                        Indicator_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IndicatorGroup_ID, t.Indicator_ID })
                .ForeignKey("dbo.IndicatorGroups", t => t.IndicatorGroup_ID, cascadeDelete: true)
                .ForeignKey("dbo.Indicators", t => t.Indicator_ID, cascadeDelete: true)
                .Index(t => t.IndicatorGroup_ID)
                .Index(t => t.Indicator_ID);
            
            CreateTable(
                "dbo.MyClaimUserGroups",
                c => new
                    {
                        MyClaim_ID = c.Int(nullable: false),
                        UserGroup_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MyClaim_ID, t.UserGroup_ID })
                .ForeignKey("dbo.MyClaims", t => t.MyClaim_ID, cascadeDelete: true)
                .ForeignKey("dbo.UserGroups", t => t.UserGroup_ID, cascadeDelete: true)
                .Index(t => t.MyClaim_ID)
                .Index(t => t.UserGroup_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MyClaimUserGroups", "UserGroup_ID", "dbo.UserGroups");
            DropForeignKey("dbo.MyClaimUserGroups", "MyClaim_ID", "dbo.MyClaims");
            DropForeignKey("dbo.AspNetUsers", "UserGroupID", "dbo.UserGroups");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.GeoAreas", "GeoAreaBundle_ID", "dbo.GeoAreaBundles");
            DropForeignKey("dbo.GeoAreas", "GeoAreaID", "dbo.GeoAreas");
            DropForeignKey("dbo.Indicators", "IndicatorTypeID", "dbo.IndicatorTypes");
            DropForeignKey("dbo.Indicators", "IndicatorID", "dbo.Indicators");
            DropForeignKey("dbo.IndicatorGroupIndicators", "Indicator_ID", "dbo.Indicators");
            DropForeignKey("dbo.IndicatorGroupIndicators", "IndicatorGroup_ID", "dbo.IndicatorGroups");
            DropForeignKey("dbo.Cases", "PeriodID", "dbo.Periods");
            DropForeignKey("dbo.CaseIndicators", "Indicator_ID", "dbo.Indicators");
            DropForeignKey("dbo.CaseIndicators", "Case_ID", "dbo.Cases");
            DropForeignKey("dbo.DataSources", "PeriodID", "dbo.Periods");
            DropForeignKey("dbo.DataSources", "EntityID", "dbo.Entities");
            DropForeignKey("dbo.Equations", "IndicatorID", "dbo.Indicators");
            DropForeignKey("dbo.EquationElements", "Element_ID", "dbo.Elements");
            DropForeignKey("dbo.EquationElements", "Equation_ID", "dbo.Equations");
            DropForeignKey("dbo.Elements", "DataSourceID", "dbo.DataSources");
            DropForeignKey("dbo.DataSources", "DataSourceTypeID", "dbo.DataSourceTypes");
            DropForeignKey("dbo.DataSourceGroupDataSources", "DataSource_ID", "dbo.DataSources");
            DropForeignKey("dbo.DataSourceGroupDataSources", "DataSourceGroup_ID", "dbo.DataSourceGroups");
            DropForeignKey("dbo.EntityCases", "Case_ID", "dbo.Cases");
            DropForeignKey("dbo.EntityCases", "Entity_ID", "dbo.Entities");
            DropForeignKey("dbo.CaseYearIndicators", "IndicatorID", "dbo.Indicators");
            DropForeignKey("dbo.CaseYearIndicators", "CaseYearID", "dbo.CaseYears");
            DropForeignKey("dbo.CaseYears", "CaseID", "dbo.Cases");
            DropForeignKey("dbo.Indicators", "BundleID", "dbo.Bundles");
            DropIndex("dbo.MyClaimUserGroups", new[] { "UserGroup_ID" });
            DropIndex("dbo.MyClaimUserGroups", new[] { "MyClaim_ID" });
            DropIndex("dbo.IndicatorGroupIndicators", new[] { "Indicator_ID" });
            DropIndex("dbo.IndicatorGroupIndicators", new[] { "IndicatorGroup_ID" });
            DropIndex("dbo.CaseIndicators", new[] { "Indicator_ID" });
            DropIndex("dbo.CaseIndicators", new[] { "Case_ID" });
            DropIndex("dbo.EquationElements", new[] { "Element_ID" });
            DropIndex("dbo.EquationElements", new[] { "Equation_ID" });
            DropIndex("dbo.DataSourceGroupDataSources", new[] { "DataSource_ID" });
            DropIndex("dbo.DataSourceGroupDataSources", new[] { "DataSourceGroup_ID" });
            DropIndex("dbo.EntityCases", new[] { "Case_ID" });
            DropIndex("dbo.EntityCases", new[] { "Entity_ID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "UserGroupID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.GeoAreas", new[] { "GeoAreaBundle_ID" });
            DropIndex("dbo.GeoAreas", new[] { "GeoAreaID" });
            DropIndex("dbo.Equations", new[] { "IndicatorID" });
            DropIndex("dbo.Elements", new[] { "DataSourceID" });
            DropIndex("dbo.DataSources", new[] { "EntityID" });
            DropIndex("dbo.DataSources", new[] { "PeriodID" });
            DropIndex("dbo.DataSources", new[] { "DataSourceTypeID" });
            DropIndex("dbo.CaseYearIndicators", new[] { "IndicatorID" });
            DropIndex("dbo.CaseYearIndicators", new[] { "CaseYearID" });
            DropIndex("dbo.CaseYears", new[] { "CaseID" });
            DropIndex("dbo.Cases", new[] { "PeriodID" });
            DropIndex("dbo.Indicators", new[] { "BundleID" });
            DropIndex("dbo.Indicators", new[] { "IndicatorTypeID" });
            DropIndex("dbo.Indicators", new[] { "IndicatorID" });
            DropTable("dbo.MyClaimUserGroups");
            DropTable("dbo.IndicatorGroupIndicators");
            DropTable("dbo.CaseIndicators");
            DropTable("dbo.EquationElements");
            DropTable("dbo.DataSourceGroupDataSources");
            DropTable("dbo.EntityCases");
            DropTable("dbo.MyClaims");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserGroups");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.GeoAreas");
            DropTable("dbo.GeoAreaBundles");
            DropTable("dbo.IndicatorTypes");
            DropTable("dbo.IndicatorGroups");
            DropTable("dbo.Periods");
            DropTable("dbo.Equations");
            DropTable("dbo.Elements");
            DropTable("dbo.DataSourceTypes");
            DropTable("dbo.DataSourceGroups");
            DropTable("dbo.DataSources");
            DropTable("dbo.Entities");
            DropTable("dbo.CaseYearIndicators");
            DropTable("dbo.CaseYears");
            DropTable("dbo.Cases");
            DropTable("dbo.Indicators");
            DropTable("dbo.Bundles");
        }
    }
}

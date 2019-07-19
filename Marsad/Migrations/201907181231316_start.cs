namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class start : DbMigration
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
                        Description = c.String(),
                        BundleID = c.Int(nullable: false),
                        MeasureUnit = c.String(),
                        IndicatorID = c.Int(),
                        IndicatorTypeID = c.Int(nullable: false),
                        TargetMillCorrelation = c.String(),
                        Importance = c.String(),
                        ApplyLevel = c.String(),
                        GenderCorrelation = c.String(),
                        EvaluationInDevAxis = c.String(),
                        Links = c.String(),
                        RefreshMethod = c.String(),
                        References = c.String(),
                        CalculationMethod = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Bundles", t => t.BundleID, cascadeDelete: false)
                .ForeignKey("dbo.IndicatorTypes", t => t.IndicatorTypeID, cascadeDelete: false)
                .ForeignKey("dbo.Indicators", t => t.IndicatorID)
                .Index(t => t.BundleID)
                .Index(t => t.IndicatorID)
                .Index(t => t.IndicatorTypeID);
            
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
                .ForeignKey("dbo.CaseYears", t => t.CaseYearID, cascadeDelete: false)
                .ForeignKey("dbo.Indicators", t => t.IndicatorID, cascadeDelete: false)
                .Index(t => t.CaseYearID)
                .Index(t => t.IndicatorID);
            
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
                .ForeignKey("dbo.Cases", t => t.CaseID, cascadeDelete: false)
                .Index(t => t.CaseID);
            
            CreateTable(
                "dbo.Cases",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Year = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Equations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IndicatorID = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        EquationText = c.String(),
                        Name = c.String(nullable: false, maxLength: 255),
                        MeasureUnit = c.String(maxLength: 255),
                        DataSourceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DataSources", t => t.DataSourceID, cascadeDelete: false)
                .ForeignKey("dbo.Indicators", t => t.IndicatorID, cascadeDelete: false)
                .Index(t => t.IndicatorID)
                .Index(t => t.DataSourceID);
            
            CreateTable(
                "dbo.DataSources",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        PublishDate = c.DateTime(nullable: false),
                        PublishNumber = c.String(),
                        PublisherName = c.String(),
                        AuthorName = c.String(),
                        OtherDataSourceType = c.String(),
                        IsPeriodic = c.Boolean(nullable: false),
                        PeriodID = c.Int(),
                        DataSourceID = c.Int(),
                        IsPart = c.Boolean(nullable: false),
                        IsHijri = c.Boolean(nullable: false),
                        DataSourceType_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DataSources", t => t.DataSourceID)
                .ForeignKey("dbo.Periods", t => t.PeriodID)
                .ForeignKey("dbo.DataSourceTypes", t => t.DataSourceType_ID)
                .Index(t => t.PeriodID)
                .Index(t => t.DataSourceID)
                .Index(t => t.DataSourceType_ID);
            
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
                .ForeignKey("dbo.DataSources", t => t.DataSourceID, cascadeDelete: false)
                .Index(t => t.DataSourceID);
            
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
                        Indicator_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Indicators", t => t.Indicator_ID)
                .Index(t => t.Indicator_ID);
            
            CreateTable(
                "dbo.IndicatorTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DataSourceTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
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
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DataSourceGroupDataSources",
                c => new
                    {
                        DataSourceGroup_ID = c.Int(nullable: false),
                        DataSource_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DataSourceGroup_ID, t.DataSource_ID })
                .ForeignKey("dbo.DataSourceGroups", t => t.DataSourceGroup_ID, cascadeDelete: false)
                .ForeignKey("dbo.DataSources", t => t.DataSource_ID, cascadeDelete: false)
                .Index(t => t.DataSourceGroup_ID)
                .Index(t => t.DataSource_ID);
            
            CreateTable(
                "dbo.ElementEquations",
                c => new
                    {
                        Element_ID = c.Int(nullable: false),
                        Equation_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Element_ID, t.Equation_ID })
                .ForeignKey("dbo.Elements", t => t.Element_ID, cascadeDelete: false)
                .ForeignKey("dbo.Equations", t => t.Equation_ID, cascadeDelete: false)
                .Index(t => t.Element_ID)
                .Index(t => t.Equation_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DataSources", "DataSourceType_ID", "dbo.DataSourceTypes");
            DropForeignKey("dbo.Indicators", "IndicatorID", "dbo.Indicators");
            DropForeignKey("dbo.Indicators", "IndicatorTypeID", "dbo.IndicatorTypes");
            DropForeignKey("dbo.IndicatorGroups", "Indicator_ID", "dbo.Indicators");
            DropForeignKey("dbo.Equations", "IndicatorID", "dbo.Indicators");
            DropForeignKey("dbo.Equations", "DataSourceID", "dbo.DataSources");
            DropForeignKey("dbo.DataSources", "PeriodID", "dbo.Periods");
            DropForeignKey("dbo.DataSources", "DataSourceID", "dbo.DataSources");
            DropForeignKey("dbo.ElementEquations", "Equation_ID", "dbo.Equations");
            DropForeignKey("dbo.ElementEquations", "Element_ID", "dbo.Elements");
            DropForeignKey("dbo.Elements", "DataSourceID", "dbo.DataSources");
            DropForeignKey("dbo.DataSourceGroupDataSources", "DataSource_ID", "dbo.DataSources");
            DropForeignKey("dbo.DataSourceGroupDataSources", "DataSourceGroup_ID", "dbo.DataSourceGroups");
            DropForeignKey("dbo.CaseYearIndicators", "IndicatorID", "dbo.Indicators");
            DropForeignKey("dbo.CaseYearIndicators", "CaseYearID", "dbo.CaseYears");
            DropForeignKey("dbo.CaseYears", "CaseID", "dbo.Cases");
            DropForeignKey("dbo.Indicators", "BundleID", "dbo.Bundles");
            DropIndex("dbo.ElementEquations", new[] { "Equation_ID" });
            DropIndex("dbo.ElementEquations", new[] { "Element_ID" });
            DropIndex("dbo.DataSourceGroupDataSources", new[] { "DataSource_ID" });
            DropIndex("dbo.DataSourceGroupDataSources", new[] { "DataSourceGroup_ID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.IndicatorGroups", new[] { "Indicator_ID" });
            DropIndex("dbo.Elements", new[] { "DataSourceID" });
            DropIndex("dbo.DataSources", new[] { "DataSourceType_ID" });
            DropIndex("dbo.DataSources", new[] { "DataSourceID" });
            DropIndex("dbo.DataSources", new[] { "PeriodID" });
            DropIndex("dbo.Equations", new[] { "DataSourceID" });
            DropIndex("dbo.Equations", new[] { "IndicatorID" });
            DropIndex("dbo.CaseYears", new[] { "CaseID" });
            DropIndex("dbo.CaseYearIndicators", new[] { "IndicatorID" });
            DropIndex("dbo.CaseYearIndicators", new[] { "CaseYearID" });
            DropIndex("dbo.Indicators", new[] { "IndicatorTypeID" });
            DropIndex("dbo.Indicators", new[] { "IndicatorID" });
            DropIndex("dbo.Indicators", new[] { "BundleID" });
            DropTable("dbo.ElementEquations");
            DropTable("dbo.DataSourceGroupDataSources");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.DataSourceTypes");
            DropTable("dbo.IndicatorTypes");
            DropTable("dbo.IndicatorGroups");
            DropTable("dbo.Periods");
            DropTable("dbo.Elements");
            DropTable("dbo.DataSourceGroups");
            DropTable("dbo.DataSources");
            DropTable("dbo.Equations");
            DropTable("dbo.Cases");
            DropTable("dbo.CaseYears");
            DropTable("dbo.CaseYearIndicators");
            DropTable("dbo.Indicators");
            DropTable("dbo.Bundles");
        }
    }
}

namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ElementYearVaules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ElementYearValues",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ElementID = c.Int(nullable: false),
                        Value = c.Single(nullable: false),
                        Year = c.Int(nullable: false),
                        ApplicationUserID = c.String(maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserID)
                .ForeignKey("dbo.Elements", t => t.ElementID, cascadeDelete: true)
                .Index(t => t.ElementID)
                .Index(t => t.ApplicationUserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ElementYearValues", "ElementID", "dbo.Elements");
            DropForeignKey("dbo.ElementYearValues", "ApplicationUserID", "dbo.AspNetUsers");
            DropIndex("dbo.ElementYearValues", new[] { "ApplicationUserID" });
            DropIndex("dbo.ElementYearValues", new[] { "ElementID" });
            DropTable("dbo.ElementYearValues");
        }
    }
}

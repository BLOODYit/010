namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserElements : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserBundles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserBundles", "Bundle_ID", "dbo.Bundles");
            DropForeignKey("dbo.ApplicationUserIndicators", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserIndicators", "Indicator_ID", "dbo.Indicators");
            DropIndex("dbo.ApplicationUserBundles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserBundles", new[] { "Bundle_ID" });
            DropIndex("dbo.ApplicationUserIndicators", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserIndicators", new[] { "Indicator_ID" });
            AddColumn("dbo.Elements", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "Indicator_ID", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Bundle_ID", c => c.Int());
            CreateIndex("dbo.Elements", "ApplicationUser_Id");
            CreateIndex("dbo.AspNetUsers", "Indicator_ID");
            CreateIndex("dbo.AspNetUsers", "Bundle_ID");
            AddForeignKey("dbo.Elements", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "Indicator_ID", "dbo.Indicators", "ID");
            AddForeignKey("dbo.AspNetUsers", "Bundle_ID", "dbo.Bundles", "ID");
            DropTable("dbo.ApplicationUserBundles");
            DropTable("dbo.ApplicationUserIndicators");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserIndicators",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Indicator_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Indicator_ID });
            
            CreateTable(
                "dbo.ApplicationUserBundles",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Bundle_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Bundle_ID });
            
            DropForeignKey("dbo.AspNetUsers", "Bundle_ID", "dbo.Bundles");
            DropForeignKey("dbo.AspNetUsers", "Indicator_ID", "dbo.Indicators");
            DropForeignKey("dbo.Elements", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "Bundle_ID" });
            DropIndex("dbo.AspNetUsers", new[] { "Indicator_ID" });
            DropIndex("dbo.Elements", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "Bundle_ID");
            DropColumn("dbo.AspNetUsers", "Indicator_ID");
            DropColumn("dbo.Elements", "ApplicationUser_Id");
            CreateIndex("dbo.ApplicationUserIndicators", "Indicator_ID");
            CreateIndex("dbo.ApplicationUserIndicators", "ApplicationUser_Id");
            CreateIndex("dbo.ApplicationUserBundles", "Bundle_ID");
            CreateIndex("dbo.ApplicationUserBundles", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserIndicators", "Indicator_ID", "dbo.Indicators", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserIndicators", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserBundles", "Bundle_ID", "dbo.Bundles", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserBundles", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}

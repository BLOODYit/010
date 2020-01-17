namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersElements : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Elements", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Elements", new[] { "ApplicationUser_Id" });
            CreateTable(
                "dbo.ApplicationUserElements",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Element_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Element_ID })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Elements", t => t.Element_ID, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Element_ID);
            
            DropColumn("dbo.Elements", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Elements", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserElements", "Element_ID", "dbo.Elements");
            DropForeignKey("dbo.ApplicationUserElements", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserElements", new[] { "Element_ID" });
            DropIndex("dbo.ApplicationUserElements", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserElements");
            CreateIndex("dbo.Elements", "ApplicationUser_Id");
            AddForeignKey("dbo.Elements", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}

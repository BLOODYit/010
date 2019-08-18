namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Name = c.String(),
                        UserGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserGroups", t => t.UserGroupID, cascadeDelete: true)
                .Index(t => t.UserGroupID);
            
            AddColumn("dbo.AspNetUsers", "UserGroupID", c => c.Int(nullable: true));
            CreateIndex("dbo.AspNetUsers", "UserGroupID");
            AddForeignKey("dbo.AspNetUsers", "UserGroupID", "dbo.UserGroups", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Claims", "UserGroupID", "dbo.UserGroups");
            DropForeignKey("dbo.AspNetUsers", "UserGroupID", "dbo.UserGroups");
            DropIndex("dbo.Claims", new[] { "UserGroupID" });
            DropIndex("dbo.AspNetUsers", new[] { "UserGroupID" });
            DropColumn("dbo.AspNetUsers", "UserGroupID");
            DropTable("dbo.Claims");
            DropTable("dbo.UserGroups");
        }
    }
}

namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userFix : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.AspNetUsers", "UserGroup_ID", "dbo.UserGroups");
            //DropForeignKey("dbo.UserGroupMyClaims", "UserGroup_ID", "dbo.UserGroups");
            //DropForeignKey("dbo.UserGroupMyClaims", "MyClaim_ID", "dbo.MyClaims");
            //DropIndex("dbo.AspNetUsers", new[] { "UserGroup_ID" });
            //DropIndex("dbo.UserGroupMyClaims", new[] { "UserGroup_ID" });
            //DropIndex("dbo.UserGroupMyClaims", new[] { "MyClaim_ID" });
            AddColumn("dbo.AspNetUsers", "EntityID", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "EntityID");
            AddForeignKey("dbo.AspNetUsers", "EntityID", "dbo.Entities", "ID");
            DropColumn("dbo.AspNetUsers", "EntityName");
            //DropColumn("dbo.AspNetUsers", "UserGroup_ID");
            //DropTable("dbo.MyClaims");
            DropTable("dbo.UserGroups");
            //DropTable("dbo.UserGroupMyClaims");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserGroupMyClaims",
                c => new
                    {
                        UserGroup_ID = c.Int(nullable: false),
                        MyClaim_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserGroup_ID, t.MyClaim_ID });
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.MyClaims",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.AspNetUsers", "UserGroup_ID", c => c.Int());
            AddColumn("dbo.AspNetUsers", "EntityName", c => c.String());
            DropForeignKey("dbo.AspNetUsers", "EntityID", "dbo.Entities");
            DropIndex("dbo.AspNetUsers", new[] { "EntityID" });
            DropColumn("dbo.AspNetUsers", "EntityID");
            CreateIndex("dbo.UserGroupMyClaims", "MyClaim_ID");
            CreateIndex("dbo.UserGroupMyClaims", "UserGroup_ID");
            CreateIndex("dbo.AspNetUsers", "UserGroup_ID");
            AddForeignKey("dbo.UserGroupMyClaims", "MyClaim_ID", "dbo.MyClaims", "ID", cascadeDelete: true);
            AddForeignKey("dbo.UserGroupMyClaims", "UserGroup_ID", "dbo.UserGroups", "ID", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUsers", "UserGroup_ID", "dbo.UserGroups", "ID");
        }
    }
}

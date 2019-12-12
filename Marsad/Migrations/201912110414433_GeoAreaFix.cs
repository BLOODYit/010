namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GeoAreaFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GeoAreas", "GeoAreaBundle_ID", "dbo.GeoAreaBundles");
            DropIndex("dbo.GeoAreas", new[] { "GeoAreaBundle_ID" });
            AddColumn("dbo.GeoAreaBundles", "GeoAreaID", c => c.Int(nullable: false));
            AddColumn("dbo.GeoAreaBundles", "ChildGeoAreaID", c => c.Int(nullable: false));
            AddColumn("dbo.GeoAreaBundles", "GeoArea_ID", c => c.Int());
            CreateIndex("dbo.GeoAreaBundles", "ChildGeoAreaID");
            CreateIndex("dbo.GeoAreaBundles", "GeoArea_ID");
            AddForeignKey("dbo.GeoAreaBundles", "ChildGeoAreaID", "dbo.GeoAreas", "ID", cascadeDelete: true);
            AddForeignKey("dbo.GeoAreaBundles", "GeoArea_ID", "dbo.GeoAreas", "ID");
            DropColumn("dbo.GeoAreas", "GeoAreaBundle_ID");
            DropColumn("dbo.GeoAreaBundles", "Code");
            DropColumn("dbo.GeoAreaBundles", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GeoAreaBundles", "Name", c => c.String(nullable: false));
            AddColumn("dbo.GeoAreaBundles", "Code", c => c.String(nullable: false));
            AddColumn("dbo.GeoAreas", "GeoAreaBundle_ID", c => c.Int());
            DropForeignKey("dbo.GeoAreaBundles", "GeoArea_ID", "dbo.GeoAreas");
            DropForeignKey("dbo.GeoAreaBundles", "ChildGeoAreaID", "dbo.GeoAreas");
            DropIndex("dbo.GeoAreaBundles", new[] { "GeoArea_ID" });
            DropIndex("dbo.GeoAreaBundles", new[] { "ChildGeoAreaID" });
            DropColumn("dbo.GeoAreaBundles", "GeoArea_ID");
            DropColumn("dbo.GeoAreaBundles", "ChildGeoAreaID");
            DropColumn("dbo.GeoAreaBundles", "GeoAreaID");
            CreateIndex("dbo.GeoAreas", "GeoAreaBundle_ID");
            AddForeignKey("dbo.GeoAreas", "GeoAreaBundle_ID", "dbo.GeoAreaBundles", "ID");
        }
    }
}

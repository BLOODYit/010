namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLimits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndicatorLimits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IndicatorID = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        IntHigh = c.Double(),
                        IntLow = c.Double(),
                        LocHigh = c.Double(),
                        LocLow = c.Double(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Indicators", t => t.IndicatorID, cascadeDelete: true)
                .Index(t => t.IndicatorID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IndicatorLimits", "IndicatorID", "dbo.Indicators");
            DropIndex("dbo.IndicatorLimits", new[] { "IndicatorID" });
            DropTable("dbo.IndicatorLimits");
        }
    }
}

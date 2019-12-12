namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finalFix2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IndicatorGroups", "Code", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.IndicatorGroups", "Code", c => c.String(nullable: false, maxLength: 255));
        }
    }
}

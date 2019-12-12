namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finalFix1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Elements", "Code", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Elements", "Code", c => c.String(nullable: false, maxLength: 255));
        }
    }
}

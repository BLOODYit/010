namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class elementValues : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ElementValues", "EquationYearID", c => c.Int(nullable: false));
            CreateIndex("dbo.ElementValues", "EquationYearID");
            AddForeignKey("dbo.ElementValues", "EquationYearID", "dbo.EquationYears", "ID", cascadeDelete: false);
            DropColumn("dbo.ElementValues", "Year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ElementValues", "Year", c => c.Int(nullable: false));
            DropForeignKey("dbo.ElementValues", "EquationYearID", "dbo.EquationYears");
            DropIndex("dbo.ElementValues", new[] { "EquationYearID" });
            DropColumn("dbo.ElementValues", "EquationYearID");
        }
    }
}

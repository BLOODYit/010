namespace Marsad.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class equationelemens : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EquationElements", "Equation_ID", "dbo.Equations");
            DropForeignKey("dbo.EquationElements", "Element_ID", "dbo.Elements");
            DropIndex("dbo.EquationElements", new[] { "Equation_ID" });
            DropIndex("dbo.EquationElements", new[] { "Element_ID" });
            DropTable("dbo.EquationElements");
            CreateTable(
                "dbo.EquationElements",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EquationID = c.Int(nullable: false),
                        ElementID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Elements", t => t.ElementID, cascadeDelete: true)
                .ForeignKey("dbo.Equations", t => t.EquationID, cascadeDelete: true)
                .Index(t => t.EquationID)
                .Index(t => t.ElementID);
            
            
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EquationElements",
                c => new
                    {
                        Equation_ID = c.Int(nullable: false),
                        Element_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Equation_ID, t.Element_ID });
            
            DropForeignKey("dbo.EquationElements", "EquationID", "dbo.Equations");
            DropForeignKey("dbo.EquationElements", "ElementID", "dbo.Elements");
            DropIndex("dbo.EquationElements", new[] { "ElementID" });
            DropIndex("dbo.EquationElements", new[] { "EquationID" });
            DropTable("dbo.EquationElements");
            CreateIndex("dbo.EquationElements", "Element_ID");
            CreateIndex("dbo.EquationElements", "Equation_ID");
            AddForeignKey("dbo.EquationElements", "Element_ID", "dbo.Elements", "ID", cascadeDelete: true);
            AddForeignKey("dbo.EquationElements", "Equation_ID", "dbo.Equations", "ID", cascadeDelete: true);
        }
    }
}

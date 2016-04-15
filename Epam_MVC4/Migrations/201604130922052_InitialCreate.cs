namespace Epam_MVC4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataRecord",
                c => new
                    {
                        TradeDate = c.DateTime(nullable: false),
                        Open = c.Double(nullable: false),
                        Low = c.Double(nullable: false),
                        High = c.Double(nullable: false),
                        Close = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TradeDate);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DataRecord");
        }
    }
}

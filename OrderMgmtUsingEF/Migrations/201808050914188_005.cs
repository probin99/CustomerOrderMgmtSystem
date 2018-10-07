namespace OrderMgmtUsingEF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _005 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "CustomerName");
            DropColumn("dbo.OrderItems", "ProductName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderItems", "ProductName", c => c.String());
            AddColumn("dbo.Orders", "CustomerName", c => c.String());
        }
    }
}

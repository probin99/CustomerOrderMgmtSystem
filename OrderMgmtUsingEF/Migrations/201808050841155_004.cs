namespace OrderMgmtUsingEF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _004 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerName", c => c.String());
            AddColumn("dbo.OrderItems", "ProductName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "ProductName");
            DropColumn("dbo.Orders", "CustomerName");
        }
    }
}

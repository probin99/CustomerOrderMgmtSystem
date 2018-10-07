namespace OrderMgmtUsingEF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _003 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderStatus");
        }
    }
}

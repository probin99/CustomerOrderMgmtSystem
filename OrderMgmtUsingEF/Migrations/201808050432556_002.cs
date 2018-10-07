namespace OrderMgmtUsingEF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _002 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Products", "ProductList_OrderID", c => c.Int());
            CreateIndex("dbo.Products", "ProductList_OrderID");
            AddForeignKey("dbo.Products", "ProductList_OrderID", "dbo.Orders", "OrderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ProductList_OrderID", "dbo.Orders");
            DropIndex("dbo.Products", new[] { "ProductList_OrderID" });
            DropColumn("dbo.Products", "ProductList_OrderID");
            DropColumn("dbo.Orders", "Discriminator");
        }
    }
}

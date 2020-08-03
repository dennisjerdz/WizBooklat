namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookstocknullablebranchId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Books", "BranchId", "dbo.Branches");
            DropIndex("dbo.Books", new[] { "BranchId" });
            AlterColumn("dbo.Books", "BranchId", c => c.Int());
            CreateIndex("dbo.Books", "BranchId");
            AddForeignKey("dbo.Books", "BranchId", "dbo.Branches", "BranchId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "BranchId", "dbo.Branches");
            DropIndex("dbo.Books", new[] { "BranchId" });
            AlterColumn("dbo.Books", "BranchId", c => c.Int(nullable: false));
            CreateIndex("dbo.Books", "BranchId");
            AddForeignKey("dbo.Books", "BranchId", "dbo.Branches", "BranchId", cascadeDelete: true);
        }
    }
}

namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountBranchnavigationproperty : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AspNetUsers", "BranchId");
            AddForeignKey("dbo.AspNetUsers", "BranchId", "dbo.Branches", "BranchId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "BranchId", "dbo.Branches");
            DropIndex("dbo.AspNetUsers", new[] { "BranchId" });
        }
    }
}

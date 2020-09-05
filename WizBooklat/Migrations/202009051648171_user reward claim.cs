namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userrewardclaim : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Claims", name: "ApplicationUser_Id", newName: "UserId");
            RenameIndex(table: "dbo.Claims", name: "IX_ApplicationUser_Id", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Claims", name: "IX_UserId", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.Claims", name: "UserId", newName: "ApplicationUser_Id");
        }
    }
}

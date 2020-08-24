namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pickupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "PickUpDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "PickUpDate");
        }
    }
}

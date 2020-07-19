namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removesettingdatecreated : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Settings", "DateCreated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Settings", "DateCreated", c => c.DateTime(nullable: false));
        }
    }
}

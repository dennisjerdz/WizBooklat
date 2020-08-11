namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class returnloanedbookissevere : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "IsSevere", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "IsSevere");
        }
    }
}

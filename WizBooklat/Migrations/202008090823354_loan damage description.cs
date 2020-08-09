namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loandamagedescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "DamageDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "DamageDescription");
        }
    }
}

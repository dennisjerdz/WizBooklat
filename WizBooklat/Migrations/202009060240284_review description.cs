namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reviewdescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "ReviewDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "ReviewDescription");
        }
    }
}

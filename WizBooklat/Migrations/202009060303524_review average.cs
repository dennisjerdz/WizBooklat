namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reviewaverage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookTemplates", "ReviewAverage", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookTemplates", "ReviewAverage");
        }
    }
}

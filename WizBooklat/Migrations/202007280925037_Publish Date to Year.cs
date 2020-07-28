namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PublishDatetoYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookTemplates", "PublishYear", c => c.Short(nullable: false));
            DropColumn("dbo.BookTemplates", "PublishDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BookTemplates", "PublishDate", c => c.DateTime());
            DropColumn("dbo.BookTemplates", "PublishYear");
        }
    }
}

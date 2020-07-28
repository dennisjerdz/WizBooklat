namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullablePublishYear : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BookTemplates", "PublishYear", c => c.Short());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BookTemplates", "PublishYear", c => c.Short(nullable: false));
        }
    }
}

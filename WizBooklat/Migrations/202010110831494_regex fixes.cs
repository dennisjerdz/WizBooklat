namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class regexfixes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Branches", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Branches", "Name", c => c.String());
        }
    }
}

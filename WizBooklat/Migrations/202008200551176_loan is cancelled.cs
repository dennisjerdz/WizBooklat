namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loaniscancelled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "IsCancelled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "IsCancelled");
        }
    }
}

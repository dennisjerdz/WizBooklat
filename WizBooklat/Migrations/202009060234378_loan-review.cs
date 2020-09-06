namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loanreview : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "Review", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "Review");
        }
    }
}

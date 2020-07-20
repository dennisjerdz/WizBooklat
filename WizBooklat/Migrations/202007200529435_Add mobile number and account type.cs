namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmobilenumberandaccounttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MobileNumber", c => c.String());
            AddColumn("dbo.AspNetUsers", "MobileNumberCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "AccountType", c => c.Short(nullable: false));
            AddColumn("dbo.AspNetUsers", "AccountStatus", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AccountStatus");
            DropColumn("dbo.AspNetUsers", "AccountType");
            DropColumn("dbo.AspNetUsers", "MobileNumberCode");
            DropColumn("dbo.AspNetUsers", "MobileNumber");
        }
    }
}

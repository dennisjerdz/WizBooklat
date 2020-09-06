namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class neweditionbooktemplateid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookTemplates", "NewEditionBookTemplateId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookTemplates", "NewEditionBookTemplateId");
        }
    }
}

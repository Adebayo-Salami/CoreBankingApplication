namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addbranch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Branch", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Branch");
        }
    }
}

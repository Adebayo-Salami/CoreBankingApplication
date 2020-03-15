namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "tillAccount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "tillAccount");
        }
    }
}

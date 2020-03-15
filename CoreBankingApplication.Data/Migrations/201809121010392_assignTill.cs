namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assignTill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TellerDetails", "tillStatus", c => c.Boolean(nullable: false));
            DropColumn("dbo.TellerDetails", "CustomerID");
            DropColumn("dbo.TillLogs", "tillStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TillLogs", "tillStatus", c => c.Boolean(nullable: false));
            AddColumn("dbo.TellerDetails", "CustomerID", c => c.Int(nullable: false));
            DropColumn("dbo.TellerDetails", "tillStatus");
        }
    }
}

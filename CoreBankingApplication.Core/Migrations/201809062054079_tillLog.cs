namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tillLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TillLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        accountEntry = c.String(),
                        amount = c.Double(nullable: false),
                        dateOfTransaction = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TillLogs");
        }
    }
}

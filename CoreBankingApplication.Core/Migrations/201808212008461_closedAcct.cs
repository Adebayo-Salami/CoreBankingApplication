namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class closedAcct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CloseAccts",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        customerAccountNumber = c.String(nullable: false),
                        customerAccountType = c.String(nullable: false),
                        accountName = c.String(nullable: false),
                        accountBalance = c.Double(nullable: false),
                        balanceStatus = c.String(nullable: false),
                        branchCreated = c.String(),
                        dateCreated = c.DateTime(nullable: false),
                        interestRate = c.Double(nullable: false),
                        lien = c.Double(nullable: false),
                        cot = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CloseAccts");
        }
    }
}

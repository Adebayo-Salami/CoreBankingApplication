namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loanPay : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.loanPayments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        accountNumber = c.String(nullable: false),
                        accountType = c.String(nullable: false),
                        amountP = c.Double(nullable: false),
                        accountToCredit = c.String(nullable: false),
                        transactionDescription = c.String(nullable: false, maxLength: 100),
                        tillAccount = c.String(nullable: false),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.loanPayments");
        }
    }
}

namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class interestAccrual : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoanInterestAccruals",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        customerAcctName = c.String(),
                        customerAcctNo = c.String(),
                        linkedAcctName = c.String(),
                        linkedAcctNo = c.String(),
                        accruedAmt = c.Double(nullable: false),
                        dateOfLoan = c.DateTime(nullable: false),
                        duration = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LoanInterestAccruals");
        }
    }
}

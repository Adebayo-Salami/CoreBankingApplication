namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class postWithdrawal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostWithdrawals",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        accountNumber = c.String(nullable: false),
                        accountType = c.String(nullable: false),
                        amountW = c.Double(nullable: false),
                        accountToCredit = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PostWithdrawals");
        }
    }
}

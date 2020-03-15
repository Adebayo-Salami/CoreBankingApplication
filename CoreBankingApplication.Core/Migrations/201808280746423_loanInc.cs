namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loanInc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.loanIncomes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        accountName = c.String(),
                        acctNo = c.String(),
                        incomeG = c.String(),
                        date = c.DateTime(nullable: false),
                        duration = c.String(),
                        rate = c.String(),
                        amountBorrowed = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.loanIncomes");
        }
    }
}

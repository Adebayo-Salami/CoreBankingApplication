namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FundGL : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FundCapitalGLs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        glSending = c.String(),
                        glReceiving = c.String(nullable: false),
                        amountDebit = c.Double(nullable: false),
                        amountCredit = c.Double(nullable: false),
                        narration = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FundCapitalGLs");
        }
    }
}

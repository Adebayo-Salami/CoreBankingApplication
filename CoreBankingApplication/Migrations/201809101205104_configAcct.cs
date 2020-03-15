namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class configAcct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountTypeConfigs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        accountType = c.String(nullable: false),
                        interestRate = c.Double(nullable: false),
                        lien = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccountTypeConfigs");
        }
    }
}

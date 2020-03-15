namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customerAccounts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerAccounts",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        customerAccountNumber = c.String(),
                        customerAccountType = c.String(),
                        accountBalance = c.Double(nullable: false),
                        dateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CustomerAccounts");
        }
    }
}

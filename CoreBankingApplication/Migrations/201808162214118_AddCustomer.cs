namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        customerName = c.String(nullable: false, maxLength: 100),
                        customerPhone = c.String(nullable: false, maxLength: 20),
                        customerEmail = c.String(nullable: false, maxLength: 50),
                        customerLocation = c.String(nullable: false, maxLength: 100),
                        customerNationalId = c.String(maxLength: 100),
                        customerVoterId = c.String(maxLength: 100),
                        customerElectricityId = c.String(maxLength: 100),
                        accountNumber = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Customers");
        }
    }
}

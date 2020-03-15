namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tillDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TellerDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tellerUsername = c.String(),
                        tillAccountNumber = c.Int(nullable: false),
                        tillBalance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TellerDetails");
        }
    }
}

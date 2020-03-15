namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onUsWithdraw : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.On_Us_Withdrawal",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        terminal_ID = c.Int(nullable: false),
                        location = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.On_Us_Withdrawal");
        }
    }
}

namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EodStatus",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EodStatus");
        }
    }
}

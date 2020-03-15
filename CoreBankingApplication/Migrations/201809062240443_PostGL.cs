namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostGL : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostGLTransactions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        glSending = c.String(nullable: false),
                        glReceiving = c.String(nullable: false),
                        amountDebit = c.Double(nullable: false),
                        amountCredit = c.Double(nullable: false),
                        narration = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PostGLTransactions");
        }
    }
}

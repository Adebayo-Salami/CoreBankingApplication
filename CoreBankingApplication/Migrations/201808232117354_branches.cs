namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class branches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.branchDBs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        branchName = c.String(nullable: false),
                        branchLoc = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.branchDBs");
        }
    }
}

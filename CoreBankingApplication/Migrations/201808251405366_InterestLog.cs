namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InterestLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InterestLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        acctEntry = c.String(),
                        correspondEntry = c.String(),
                        entryAccN = c.String(),
                        amt = c.Double(nullable: false),
                        Entrydesc = c.String(),
                        dateTr = c.DateTime(nullable: false),
                        recordFor = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InterestLogs");
        }
    }
}

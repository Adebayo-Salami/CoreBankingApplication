namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cotIncome : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CotIncomeGLs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        acctEntry = c.String(),
                        correspondEntry = c.String(),
                        entryAccN = c.String(),
                        customerAccNumb = c.String(),
                        customerAcctype = c.String(),
                        amt = c.Double(nullable: false),
                        Entrydesc = c.String(),
                        dateTr = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CotIncomeGLs");
        }
    }
}

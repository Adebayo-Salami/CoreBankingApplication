namespace CoreBankingApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndex : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountIndexes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        accountName = c.String(nullable: false, maxLength: 100),
                        accountType = c.Byte(nullable: false),
                        glCategory_id = c.Byte(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.GlCategories", t => t.glCategory_id)
                .Index(t => t.glCategory_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountIndexes", "glCategory_id", "dbo.GlCategories");
            DropIndex("dbo.AccountIndexes", new[] { "glCategory_id" });
            DropTable("dbo.AccountIndexes");
        }
    }
}

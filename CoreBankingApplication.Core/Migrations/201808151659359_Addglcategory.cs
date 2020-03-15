namespace CoreBankingApplication.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addglcategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GlCategories",
                c => new
                    {
                        id = c.Byte(nullable: false),
                        type = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GlCategories");
        }
    }
}

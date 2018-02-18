namespace Notifix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "login", c => c.String(nullable: false));
            AlterColumn("dbo.User", "password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "password", c => c.String());
            AlterColumn("dbo.User", "login", c => c.String());
        }
    }
}

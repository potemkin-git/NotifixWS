namespace Notifix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notification : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notification", "type", c => c.String(nullable: false));
            AlterColumn("dbo.User", "first_name", c => c.String(nullable: false));
            AlterColumn("dbo.User", "last_name", c => c.String(nullable: false));
            AlterColumn("dbo.User", "avatar_src", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "avatar_src", c => c.String());
            AlterColumn("dbo.User", "last_name", c => c.String());
            AlterColumn("dbo.User", "first_name", c => c.String());
            AlterColumn("dbo.Notification", "type", c => c.String());
        }
    }
}

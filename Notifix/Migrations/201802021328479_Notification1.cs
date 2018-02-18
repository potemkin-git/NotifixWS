namespace Notifix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notification1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notification", "user_id", "dbo.User");
            DropIndex("dbo.Notification", new[] { "user_id" });
            AlterColumn("dbo.Notification", "user_id", c => c.Int(nullable: false));
            CreateIndex("dbo.Notification", "user_id");
            AddForeignKey("dbo.Notification", "user_id", "dbo.User", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notification", "user_id", "dbo.User");
            DropIndex("dbo.Notification", new[] { "user_id" });
            AlterColumn("dbo.Notification", "user_id", c => c.Int());
            CreateIndex("dbo.Notification", "user_id");
            AddForeignKey("dbo.Notification", "user_id", "dbo.User", "id");
        }
    }
}

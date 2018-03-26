namespace Notifix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        type = c.String(nullable: false),
                        description = c.String(),
                        creationDate = c.DateTime(nullable: false),
                        exp_date = c.DateTime(nullable: false),
                        latitude = c.Single(nullable: false),
                        longitude = c.Single(nullable: false),
                        nb_conf = c.Int(nullable: false),
                        nd_deny = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.user_id, cascadeDelete: true)
                .Index(t => t.user_id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        first_name = c.String(nullable: false),
                        last_name = c.String(nullable: false),
                        login = c.String(nullable: false),
                        email = c.String(nullable: false),
                        password = c.String(nullable: false),
                        address = c.String(),
                        city = c.String(),
                        avatar_src = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notification", "user_id", "dbo.User");
            DropIndex("dbo.Notification", new[] { "user_id" });
            DropTable("dbo.User");
            DropTable("dbo.Notification");
        }
    }
}

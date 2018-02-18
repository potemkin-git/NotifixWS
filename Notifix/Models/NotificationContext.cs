namespace Notifix.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class NotificationContext : DbContext, IDisposable
    {
        // Your context has been configured to use a 'NotificationContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Notifix.Models.NotificationContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'NotificationContext' 
        // connection string in the application configuration file.
        public NotificationContext()
            : base("name=NotifixDB")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Notification> notificationList { get; set; }
    }
}
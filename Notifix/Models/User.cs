using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Notifix.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int id { get; set; }

        public ICollection<Notification> notificationList { get; set; }

        [Column("first_name")]
        [Required]
        public string firstName { get; set; }

        [Column("last_name")]
        [Required]
        public string lastName { get; set; }

        [Column("login")]
        [Required]
        public string login { get; set; }

        [Column("password")]
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Column("address")]
        public string address { get; set; }

        [Column("city")]
        public string city { get; set; }

        [Column("avatar_src")]
        [Required]
        public string avatarSrc { get; set; }

    }
}
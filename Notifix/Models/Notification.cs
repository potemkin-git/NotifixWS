using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Notifix.Models
{
    [Table("Notification")]
    public class Notification
    {
        public Notification()
        {
            creationDate = DateTime.Now;
        }

        [Key]
        public int id { get; set; }

        [Column("user_id")]
        [Required]
        public User user { get; set; }

        [Column("type")]
        [Required]
        public string type { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Required]
        public DateTime creationDate { get; set; }

        [Column("exp_date")]
        [Required]
        public DateTime expirationDate { get; set; }

        [Column("latitude")]
        [Required]
        public float latitude { get; set; }

        [Column("longitude")]
        [Required]
        public float longitude { get; set; }

        [Column("nb_conf")]
        public int nbConf { get; set; }

        [Column("nd_deny")]
        public int nbDeny { get; set; }
    }
}
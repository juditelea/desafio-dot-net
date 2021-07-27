using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppTmDb.Models
{
    [Table("Filmes")]
    public class Filme
    {
        
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        [Column("NomeFilme")]
        [Required]
        [StringLength(100)]
        public string NomeFilme { get; set; }

        [Column("LinkImg")]
        [Required]
        [StringLength(250)]
        public string LinkImg { get; set; }
    
    }
}

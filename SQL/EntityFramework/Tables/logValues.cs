using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptimaValue
{
    public class logValues
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public decimal index { get; set; }

        public tagConfig tag { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime logTime { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string value { get; set; }

        [Column(TypeName = "float")]
        public float numericValue { get; set; }
    }
}

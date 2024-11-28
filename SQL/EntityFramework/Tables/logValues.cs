using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptimaValue
{
    public class logValues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal index { get; set; }

        // Foreign Key till tagConfig
        [ForeignKey("tag")]
        public int tag_id { get; set; }

        // Navigation property till tagConfig
        public tagConfig tag { get; set; }

        [Column(TypeName = "datetime2")] // Bättre precision än datetime
        public DateTime logTime { get; set; }

        [StringLength(30)] // nvarchar(30)
        public string value { get; set; }

        public float? numericValue { get; set; } // Float är redan rätt datatyp i EF Core

        [StringLength(10)] // nvarchar(10)
        public string opcQuality { get; set; }
    }
}

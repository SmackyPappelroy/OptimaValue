using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptimaValue
{
    public class plcConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public bool active { get; set; } // `bit` hanteras automatiskt av EF Core för bool

        [Required] // Gör kolumnen obligatorisk
        [MaxLength(255)] // Begränsa till maximalt 255 tecken (standard för EF Core nvarchar)
        public string name { get; set; }

        [Required]
        [MaxLength(200)]
        public string ipAddress { get; set; }

        [MaxLength(10)]
        public string cpuType { get; set; }

        public short rack { get; set; } // smallint hanteras direkt av EF Core

        public short slot { get; set; }

        public int syncTimeDbNr { get; set; }

        public int syncTimeOffset { get; set; }

        public bool syncActive { get; set; }

        [MaxLength(20)]
        public string syncBoolAddress { get; set; }

        [Column(TypeName = "datetime2")] // Bättre precision för tidsfält
        public DateTime lastSyncTime { get; set; }
    }
}

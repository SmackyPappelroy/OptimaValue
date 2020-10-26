using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptimaValue
{
    public class plcConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }

        [Column(TypeName = "bit")]
        public bool active { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string name { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string ipAddress { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(10)]
        public string cpuType { get; set; }

        [Column(TypeName = "smallint")]
        public short rack { get; set; }

        [Column(TypeName = "smallint")]
        public short slot { get; set; }

        [Column(TypeName = "int")]
        public int syncTimeDbNr { get; set; }

        [Column(TypeName = "int")]
        public int syncTimeOffset { get; set; }

        [Column(TypeName = "bit")]
        public bool syncActive { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(20)]
        public string syncBoolAddress { get; set; }

        [Column(TypeName = "datetime")]
        public System.DateTime lastSyncTime { get; set; }
    }
}

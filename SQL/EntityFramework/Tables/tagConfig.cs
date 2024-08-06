using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptimaValue
{
    public class tagConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }

        [Column(TypeName = "bit")]
        public bool active { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        //[Column(TypeName = "nvarchar")]
        //[StringLength(50)]
        public string name { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string description { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string logType { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan timeOfDay { get; set; }

        [Column(TypeName = "float")]
        public float deadband { get; set; }

        //[Column(TypeName = "nvarchar")]
        //[StringLength(50)]
        [Column(TypeName = "nvarchar(MAX)")]
        public string plcName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string varType { get; set; }

        [Column(TypeName = "int")]
        public int blockNr { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string dataType { get; set; }

        [Column(TypeName = "int")]
        public int startByte { get; set; }

        [Column(TypeName = "int")]
        public int nrOfElements { get; set; }

        [Column(TypeName = "tinyint")]
        public byte bitAddress { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string logFreq { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string tagUnit { get; set; }

        public int eventId { get; set; }

        [Column(TypeName = "bit")]
        public bool isBooleanTrigger { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string boolTrigger { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string analogTrigger { get; set; }

        [Column(TypeName = "float")]
        public float analogValue { get; set; }

        [Column(TypeName = "float")]
        public float scaleMin { get; set; }

        [Column(TypeName = "float")]
        public float scaleMax { get; set; }

        [Column(TypeName = "float")]
        public float rawMin { get; set; }

        [Column(TypeName = "float")]
        public float rawMax { get; set; }

        [Column(TypeName = "float")]
        public float scaleOffset { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(2000)]
        public string calculation { get; set; }

    }
}

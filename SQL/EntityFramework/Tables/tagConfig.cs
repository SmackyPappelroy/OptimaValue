using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptimaValue
{
    public class tagConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public bool active { get; set; } // `bit` hanteras automatiskt av EF Core för bool

        [Required] // Kolumnen är obligatorisk
        [MaxLength(255)] // Begränsa "name" till 255 tecken för att undvika onödigt nvarchar(MAX)
        public string name { get; set; }

        [MaxLength(1000)] // Begränsa beskrivningens längd för att förbättra prestanda
        public string description { get; set; }

        [MaxLength(50)]
        public string logType { get; set; }

        public TimeSpan timeOfDay { get; set; } // EF Core hanterar TimeSpan som `time`

        public float deadband { get; set; } // Float mappas automatiskt till rätt SQL-typ

        [MaxLength(255)]
        public string plcName { get; set; }

        [MaxLength(30)]
        public string varType { get; set; }

        public int blockNr { get; set; }

        [MaxLength(30)]
        public string dataType { get; set; }

        public int startByte { get; set; }

        public int nrOfElements { get; set; }

        public byte bitAddress { get; set; } // Tinyint hanteras korrekt med byte

        [MaxLength(30)]
        public string logFreq { get; set; }

        [MaxLength(30)]
        public string tagUnit { get; set; }

        public int eventId { get; set; }

        public bool isBooleanTrigger { get; set; }

        [MaxLength(30)]
        public string boolTrigger { get; set; }

        [MaxLength(30)]
        public string analogTrigger { get; set; }

        public float analogValue { get; set; }

        public float scaleMin { get; set; }

        public float scaleMax { get; set; }

        public float rawMin { get; set; }

        public float rawMax { get; set; }

        public float scaleOffset { get; set; }

        [MaxLength(2000)] // Lämna "calculation" som nvarchar(2000)
        public string calculation { get; set; }
    }
}

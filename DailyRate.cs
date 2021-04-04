namespace ExchangeRates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DailyRate")]
    public partial class DailyRate
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string IdCurrency { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "date")]
        public DateTime Dt { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "numeric")]
        public decimal Rate { get; set; }

        public virtual Currency Currency { get; set; }
    }
}

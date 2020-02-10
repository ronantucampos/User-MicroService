using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Entities.Base
{
    public class EntityBase
    {
        [Key]
        public long Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public long CriadoPor { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public long? AlteradoPor { get; set; }
        public DateTime? DataExclusao { get; set; }
        public long? ExcluidoPor { get; set; }
    }
}

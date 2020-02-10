using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Business.View
{
    public class Pager
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? NumberOfRecord { get; set; }
        public int? NumberOfPages { get; set; }
    }
}

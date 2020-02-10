using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Business.View
{
    public class ListResultView<T> : Pager
    {
        public List<T> List { get; set; }
    }
}

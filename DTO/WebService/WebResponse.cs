using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebService
{
    public class ResponseData<T>
    {
        public DateTime DateTime => DateTime.Now;

        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}

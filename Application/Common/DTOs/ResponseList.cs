using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Common.DTOs
{
    public class ResponseList<T> where T : class
    {
        public IEnumerable<T> data { get; set; } = new List<T>();
        public int totalRecords { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        
        public static ResponseList<T> Create(IEnumerable<T> data, int totalRecords, int pageNumber, int pageSize)
        {
            return new ResponseList<T>
            {
                data = data,
                totalRecords = totalRecords,
                pageNumber = pageNumber,
                pageSize = pageSize
            };
        }
    }
}
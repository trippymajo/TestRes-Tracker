using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrtShared.DTO;

namespace TrtParserService.ResultSaver
{
    class EfResultSaver : IResultSaver
    {
        public EfResultSaver() { }

        public Task<int> SaveAsync(TestRunDTO dto)
        {

        }
    }
}

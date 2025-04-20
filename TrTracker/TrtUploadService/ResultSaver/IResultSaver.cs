using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrtShared.DTO;

namespace TrtParserService.ResultSaver
{
    public interface IResultSaver
    {
        Task<int> SaveAsync(TestRunDTO dto);
    }
}

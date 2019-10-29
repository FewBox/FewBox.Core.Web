using FewBox.Core.Web.Dto;
using System;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public interface IExceptionHandler
    {
        Task<ErrorResponseDto> Handle(string name, string param);
    }
}

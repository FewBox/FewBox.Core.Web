using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Core.Web.Filter
{
    public interface IExceptionHandler
    {
        ErrorResponseDto Handle(Exception exception);
    }
}

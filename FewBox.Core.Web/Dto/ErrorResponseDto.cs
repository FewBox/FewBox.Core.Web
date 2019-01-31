using System;

namespace FewBox.Core.Web.Dto
{
    public class ErrorResponseDto : MetaResponseDto
    {
        public ErrorResponseDto(string errorMessage, string errorCode)
        {
            this.ErrorMessage = errorMessage;
            this.ErrorCode = errorCode;
            this.IsSuccessful = false;
        }

        public ErrorResponseDto(string errorMessage) : this(errorMessage, String.Empty)
        {
        }
    }
}

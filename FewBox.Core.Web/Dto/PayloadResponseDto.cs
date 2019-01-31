namespace FewBox.Core.Web.Dto
{
    public class PayloadResponseDto<T> : MetaResponseDto
    {
        public T Payload { get; set; }

        public PayloadResponseDto()
        {
            this.IsSuccessful = true;
        }
    }
}
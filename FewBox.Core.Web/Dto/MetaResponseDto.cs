namespace FewBox.Core.Web.Dto
{
    public class MetaResponseDto
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }

        public MetaResponseDto()
        {
            this.IsSuccessful = true;
        }
    }
}

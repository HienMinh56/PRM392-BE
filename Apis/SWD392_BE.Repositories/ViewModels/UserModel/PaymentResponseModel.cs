namespace SWD392_BE.Services.Services
{
    internal class PaymentResponseModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
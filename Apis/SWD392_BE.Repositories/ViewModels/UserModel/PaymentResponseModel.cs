namespace SWD392_BE.Repositories.ViewModels.UserModel
{
    internal class PaymentResponseModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
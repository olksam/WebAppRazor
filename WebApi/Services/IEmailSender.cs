namespace WebApi.Services {
    public interface IEmailSender {
        Task SendEmail(string to, string text, string title);
    }
}

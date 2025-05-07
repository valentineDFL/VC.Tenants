namespace VC.Tenants;

public class Message
{
    public Message(string subject, string text, string receiverName, string receiverMail, string header)
    {
        Subject = subject;
        Text = text;
        ReceiverName = receiverName;
        ReceiverMail = receiverMail;
        Header = header;
    }

    /// <summary>
    /// Тема письма
    /// </summary>
    public string Subject { get; private set; }

    /// <summary>
    /// Основное содержание письма
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Имя получателя
    /// </summary>
    public string ReceiverName { get; private set; }

    /// <summary>
    /// Почта получателя
    /// </summary>
    public string ReceiverMail { get; private set; }

    /// <summary>
    /// Заголовок письма
    /// </summary>
    public string Header { get; private set; }
}
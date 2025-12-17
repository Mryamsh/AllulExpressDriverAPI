using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class WhatsAppService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _from;

    public WhatsAppService(IConfiguration config)
    {
        _accountSid = config["TWILIO__ACCOUNTSID"];
        _authToken = config["TWILIO__AUTHTOKEN"];
        _from = config["TWILIO__WHATSAPPNUMBER"]; // e.g. whatsapp:+14155238886
    }

    public async Task SendOtp(string phone, string otp)
    {
        Console.WriteLine(_accountSid);

        Console.WriteLine(_authToken);

        Console.WriteLine(_from);

        if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken))
            throw new Exception("Twilio credentials missing");

        TwilioClient.Init(_accountSid, _authToken);

        await MessageResource.CreateAsync(
            from: new PhoneNumber(_from),
            to: new PhoneNumber($"whatsapp:{phone}"),
            body: $"Your OTP is {otp}"
        );
    }
}

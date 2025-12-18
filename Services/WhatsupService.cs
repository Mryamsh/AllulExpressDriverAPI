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
        _accountSid = config["TWILIO:ACCOUNTSID"];
        _authToken = config["Twilio:AuthToken"];
        _from = config["Twilio:From"]; // e.g. whatsapp:+14155238886
    }

    public async Task SendOtp(string phone, string otp)
    {
        Console.WriteLine("account SID" + _accountSid);

        Console.WriteLine("token.  " + _authToken);

        Console.WriteLine("number.  " + _from);

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

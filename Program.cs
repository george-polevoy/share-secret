using System.Security.Cryptography;
using System.Text;

while (true)
{
    Console.WriteLine("Type what you are going to do: send,receive,quit?");
    var command = Console.ReadLine();
    try
    {
        switch (command)
        {
            case "send":
                Send();
                return;
            case "receive":
                Receive();
                return;
            case "quit":
                return;
            default:
                Console.WriteLine("Unrecognized command.");
                break;
        }
    }
    catch (FormatException)
    {
        Console.WriteLine();
        Console.Error.WriteLine(
            "Format error. Check that you copy the exact values from console, without trailing space.");
        Console.WriteLine();
    }
    catch (CryptographicException)
    {
        Console.WriteLine("Cryptographic error. Check that you've copied the exact values.");
    }
}

void Send()
{
    using var rsa = RSA.Create();
    Console.WriteLine("Enter the public key from the receiving party:");
    Console.WriteLine();
    var publicKeyBase64String = Console.ReadLine();
    if (publicKeyBase64String is null)
    {
        return;
    }
    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64String), out _);
    Console.WriteLine();
    Console.WriteLine("Enter your secret:");
    Console.WriteLine();
    var plainSecret = Console.ReadLine();
    if (plainSecret is null)
    {
        return;
    }
    var encryptedSecret = rsa.Encrypt(Encoding.UTF8.GetBytes(plainSecret), RSAEncryptionPadding.Pkcs1);
    Console.WriteLine("Here is your encrypted secret:");
    Console.WriteLine();
    Console.WriteLine(Convert.ToBase64String(encryptedSecret));
}

void Receive()
{
    Console.WriteLine("Generating private key...");
    using var rsa = RSA.Create(4096);
    var publicKey = rsa.ExportSubjectPublicKeyInfo();
    var publicKeyBase64String = Convert.ToBase64String(publicKey);
    Console.WriteLine("Following is the public key you can use for receiving. Send it to the other party:");
    Console.WriteLine();
    Console.WriteLine(publicKeyBase64String);
    Console.WriteLine();
    Console.WriteLine("Enter the encrypted secret text received from the other party:");
    Console.WriteLine();
    var receivedEncodedSecret = Console.ReadLine();
    if (receivedEncodedSecret is null)
    {
        return;
    }
    var encryptedSecret = Convert.FromBase64String(receivedEncodedSecret);
    var secretBytes = rsa.Decrypt(encryptedSecret, RSAEncryptionPadding.Pkcs1);
    Console.WriteLine();
    Console.WriteLine("Here is your decoded secret:");
    Console.WriteLine();
    Console.WriteLine(Encoding.UTF8.GetString(secretBytes));
}
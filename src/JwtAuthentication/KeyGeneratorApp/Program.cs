using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

using var rsa = RSA.Create();
string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
Console.WriteLine($"-----Private RSA key-----{Environment.NewLine}{privateKey}{Environment.NewLine}");
Console.WriteLine($"-----Public RSA key-----{Environment.NewLine}{publicKey}");
Console.WriteLine($"-----Public RSA UTF8 key-----{Environment.NewLine}{Encoding.UTF8.GetString(rsa.ExportRSAPublicKey())}");

var pub = rsa.ToXmlString(false);
var priv = rsa.ToXmlString(true);

string keyFile = Path.Combine(Directory.GetCurrentDirectory(), "keys", "private.key");
await using FileStream file = File.Create(keyFile);
await file.WriteAsync(Encoding.UTF8.GetBytes(pub));
file.Close();

/*
 * Sign JWT with private key
 */
var encryptingRsa = RSA.Create();
encryptingRsa.FromXmlString(File.ReadAllText(keyFile));
var creds = new SigningCredentials(
    key: new RsaSecurityKey(encryptingRsa),
    algorithm: SecurityAlgorithms.RsaSha256
);

var claims = new List<Claim> { new("id", "3") };

var jwt = new JwtSecurityToken(
    new JwtHeader(creds),
    new JwtPayload(
        issuer: "myapp",
        audience: "webapi",
        claims,
        DateTime.UtcNow,
        DateTime.UtcNow.AddHours(5)
    )
);

string token = new JwtSecurityTokenHandler().WriteToken(jwt);

/*
 * Validate JWT with public key
 */

var decryptingRsa = RSA.Create();
decryptingRsa.FromXmlString(rsa.ToXmlString(false));
new JwtSecurityTokenHandler().ValidateToken(token,
    new TokenValidationParameters {
        IssuerSigningKey = new RsaSecurityKey(decryptingRsa),
        RequireAudience = true,
        RequireExpirationTime = true,
        ValidIssuer = "myapp",
        ValidAudience = "webapi",
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
    }, out SecurityToken s);

Console.WriteLine();



//
// using var rsa2 = RSA.Create();
// rsa2.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
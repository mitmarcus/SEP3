using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication;
using Microsoft.IdentityModel.Tokens;
using persistance.Entity;

namespace grpc.Logic;

public class JwtLogicImpl : IJwtLogic
{
    private readonly IConfiguration _config;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _validationParameters;

    public JwtLogicImpl(IConfiguration config)
    {
        _config = config;
        _tokenHandler = new JwtSecurityTokenHandler();
        _validationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = _config["Jwt:Audience"],
            ValidateLifetime = true,
        };
    }

    private List<Claim> GenerateClaims(Credential credential)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(ClaimTypes.Name, credential.Email),
            new Claim(ClaimTypes.Role, "basic"),
            new Claim("UserId", credential.UserId.ToString())
        };
        return claims.ToList();
    }

    public string GenerateJwt(Credential credential)
    {
        List<Claim> claims = GenerateClaims(credential);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var header = new JwtHeader(signIn);
        var payload = new JwtPayload(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            null,
            DateTime.UtcNow.AddMinutes(60));

        var token = new JwtSecurityToken(header, payload);

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return serializedToken;
    }

    public string ParseEmail(string jwt)
    {
       
        var token = _tokenHandler.ReadJwtToken(jwt);
        var email = token.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
        return email;
    }

    public AuthenticationEntity ParseToken(string jwt)
    {
        var token = _tokenHandler.ReadJwtToken(jwt);
        var email = token.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
        var userId = long.Parse(token.Claims.First(claim => claim.Type == "UserId").Value);
        var authLevel = token.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
        return new AuthenticationEntity(userId, email, authLevel);
    }
    
    public bool ValidateToken(string token)
    {
        try
        {
            _tokenHandler.ValidateToken(token, _validationParameters, out SecurityToken validatedToken);
        }
        catch (System.Exception)
        {
            return false;
        }

        return true;
    }
}
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaAPI.Services.Contracts
{
    public class AuthService:IAuthService
    {
        public bool ValidateLogin(string username, string password)
        {
            //aqui haríamos la validación, de momento algo de ejemplo
            if(username.Equals("usuario") && password.Equals("123456"))
                return true;
            return false;
        }

        public string GenerateToken(DateTime fechaActual, string username, TimeSpan tiempoValidez)
        {
            var fechaExpiracion = fechaActual.Add(tiempoValidez);
            //Configuramos las claims
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(fechaActual).ToUniversalTime().ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                ),
                new Claim("roles","Cliente"),
                new Claim("roles","Administrador"),
            };
            
            //Añadimos las credenciales
            var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes("G3VF4C6KFV43JH6GKCDFGJH45V36JHGV3H4C6F3GJC63HG45GH6V345GHHJ4623FJL3HCVMO1P23PZ07W8")),
                    SecurityAlgorithms.HmacSha256Signature
            );//luego se debe configurar para obtener estos valores, así como el issuer y audience desde el appsetings.json

            //Configuracion del jwt token
            var jwt = new JwtSecurityToken(
                issuer: "Peticionario",
                audience: "Public",
                claims:claims,
                notBefore:fechaActual,
                expires:fechaExpiracion,
                signingCredentials:signingCredentials
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

    }
}

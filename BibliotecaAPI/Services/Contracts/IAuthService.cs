using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.Services.Contracts
{
    public interface IAuthService
    {
        bool ValidateLogin(string username, string password);
        string GenerateToken(DateTime fechaActual, string username, TimeSpan tiempoValidez);
    }
}

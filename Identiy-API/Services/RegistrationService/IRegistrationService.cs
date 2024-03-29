﻿using Identiy_API.Model;

namespace Identiy_API.Services.RegistrationService
{
    public interface IRegistrationService
    {
        Task Registration<T>(T createManagerDTO, string role)where T: LoginDTO;
        Task<bool> IsRegistration(string email);
    }
}

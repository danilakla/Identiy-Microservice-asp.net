﻿using Identiy_API.DTO;
using Identiy_API.Model;
using Identiy_API.Model.Payload;
using Identiy_API.Services;
using Identiy_API.Services.Caching;
using Identiy_API.Services.CryptograthyService;
using Identiy_API.Services.RegistrationService;
using Identiy_API.Services.UniversityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identiy_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmEmailController : ControllerBase
    {
        private readonly IDeanService deanService;
        private readonly IUniversityService universityService;
        private readonly ITempSaveDataService tempSaveDataService;
        private readonly ITokenServices tokenServices;
        private readonly IRegistrationService registrationService;
        private readonly ICrypto crypto;

        public ConfirmEmailController(IDeanService deanService
            ,IUniversityService universityService , 
            ITempSaveDataService tempSaveDataService, 
            ITokenServices tokenServices,
            IRegistrationService registrationService,
            ICrypto crypto
            
            )
        {
            this.deanService = deanService;
            this.universityService = universityService;
            this.tempSaveDataService = tempSaveDataService;
            this.tokenServices = tokenServices;
            this.registrationService = registrationService;
            this.crypto = crypto;
        }

        [HttpGet("manager/{token}")]
        public async Task<IActionResult> ConfirmManager(string token)
        {

            try
            {

                var manager = await tempSaveDataService.GetData<CreateManagerDTO>(token);
                
                var hasAccount = await registrationService.IsRegistration(manager.Email);
                if (hasAccount)
                {
                    throw new Exception("User had account");
                }
                await registrationService.Registration(manager, "Manager");

                var userIds =await universityService.InitUniversity(manager);


                return Redirect("https://www.youtube.com/");

            }
            catch (Exception)
            {

                throw;
            }
        
        }


        [HttpGet("dean/{token}")]
        public async Task<IActionResult> ConfirmDean(string token)
        {

            try
            {

                var deanData = await tempSaveDataService.GetData<RegistrationUserDTO>(token);
                

                var hasAccount = await registrationService.IsRegistration(deanData.Email);
             
                var data=crypto.DecryptSecretString<DeanTokenRegistraion>(deanData.AuthenticationToken);
                if (data.Role != "Dean") throw new Exception("Is not correct role");
                if (hasAccount)
                {
                    throw new Exception("User had account");
                }   
                await registrationService.Registration(deanData, "Dean");

                var userIds = await deanService.InitDean(new (){  DeanTokenRegistraion=data, loginDTO=deanData});


                return Redirect("https://www.youtube.com/");

            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet("teacher/{token}")]
        public async Task<IActionResult> ConfirmTeacher(string token)
        {

            try
            {

                var teacherData = await tempSaveDataService.GetData<RegistrationUserDTO>(token);


                var hasAccount = await registrationService.IsRegistration(teacherData.Email);

                var data = crypto.DecryptSecretString<TeacherTokenRegistration>(teacherData.AuthenticationToken);
                if (data.Role != "Teacher") throw new Exception("Is not correct role");
                if (hasAccount)
                {
                    throw new Exception("User had account");
                }
                await registrationService.Registration(teacherData,"Teacher");

               var userIds = await  universityService.InitTeacher(new() { loginDTO = teacherData, UniversityId=data.UniversityId});


                return Redirect("https://www.youtube.com/");

            }
            catch (Exception)
            {

                throw;
            }

        }
    }


}

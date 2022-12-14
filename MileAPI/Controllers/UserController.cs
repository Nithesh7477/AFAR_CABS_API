using MailKit.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MileAPI.Interfaces;
using MileDALibrary.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace MileAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUserDetails")]
        //[Route("/")]
        public IActionResult GetUserInformation(string PhoneNumber, string Password)
        {
            try
            {
                List<LoginDetails> result = _userService.GetUserInformation(PhoneNumber, Password);
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if (result.Count == 0)
                {
                    return NotFound("{\"status\": \"Invalid Mobile No/Password\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpGet("GetCountryDetails")]
        //[Route("/")]
        public IActionResult GetCountryNames()
        {
            try
            {
                List<CountryDetails> result = _userService.GetCountryDetails();
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpGet("GetStateDetails")]
        //[Route("/")]
        public IActionResult GetStateDetails()
        {
            try
            {
                List<StateDetails> result = _userService.GetStateDetails();
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpGet("GetDistrictDetails")]
        //[Route("/")]
        public IActionResult GetDistrictDetails(int stateId, int countryId)
        {
            try
            {
                List<DistrictDetails> result = _userService.GetDistrictDetails(stateId, countryId);
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpGet("GetVehicleDetails")]
        //[Route("/")]
        public IActionResult GetVehicleDetails()
        {
            try
            {
                List<VehicleDetails> result = _userService.GetVehicleDetails();
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpGet("GetGenderDetails")]
        //[Route("/")]
        public IActionResult GetGenderDetails(string settingsName)
        {
            try
            {
                List<GenderDetails> result = _userService.GetGenderDetails(settingsName);
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpPost("PostUpdatedProfile/SignUpDetails")]
        public IActionResult UpdateProfileDetails([FromBody] UpdateProfile updateProfile)
        {
            string flag;
            List<ResponseStatus> result = _userService.UpdateProfileDetails(updateProfile);
            if (!result.Any())
            {
                return NotFound("{\"status\": \"Failed\"}");
            }
            flag = result[0].Error_desc;
            if (flag.Contains("Success"))
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("GetDriverDetails")]
        public IActionResult GetDriverDetails(string? phoneNumber)
        {
            try
            {
                List<DriverDetails> result = _userService.GetDriverDetails(phoneNumber);
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if (result.Count == 0)
                {
                    return NotFound("{\"status\": \"No Data Found\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpPost("SaveUserDetails")]
        public async Task<IActionResult> SaveUserDetails([FromBody] UserDetails userDetails)
        {
            string flag;
            try
            {
                List<ResponseStatus> result = await _userService.SaveUserDetails(userDetails);

                if (!result.Any())
                {
                    return NotFound("{\"status\": \"Failed\"}");
                }
                flag = result[0].Error_desc;
                if (flag.Contains("Success"))
                {
                    return Ok(result);
                }

                return NotFound(result);

            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Insertion Failed\"}");
            }
        }

        [HttpGet("ValidateUser")]
        public async Task<IActionResult> ValidateUser(string emailId)
        {
            try
            {
                int otp = RandomNumberGenerator.GetInt32(100000, 999999);
                int result = _userService.SendEmail(emailId, otp);
                if (result == 1)
                {
                    return Ok(otp);
                }
                else
                {
                    return NotFound("{\"status\": \"Failed To Send OTP\"}");
                }
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Failed To Send OTP\"}");
            }
        }

        [HttpGet("GetUpdatedProfile")]
        public IActionResult GetUpdatedProfile(int userId)
        {
            try
            {
                List<LoginDetails> result = _userService.GetUpdatedProfile(userId);
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if (result.Count == 0)
                {
                    return NotFound("{\"status\": \"Not Found\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpPost("SaveLocation")]
        public async Task<IActionResult> SaveLocation([FromBody] LocationDetails locationDetails)
        {
            string flag;
            try
            {
                List<ResponseStatus> result = _userService.SaveLocation(locationDetails);

                if (!result.Any())
                {
                    return NotFound("{\"status\": \"Failed\"}");
                }
                flag = result[0].Error_desc;
                if (flag.Contains("Success"))
                {
                    return Ok(result);
                }

                return NotFound(result);

            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Insertion Failed\"}");
            }
        }

        [HttpGet("GetSavedLocation")]
        //[Route("/")]
        public IActionResult GetSavedLocation()
        {
            try
            {
                List<LocationDetails> result = _userService.GetSavedLocation();
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if (result.Count == 0)
                {
                    return NotFound("{\"status\": \"No Data Found\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpPost("SaveBookingDetails")]
        public async Task<IActionResult> SaveBookingDetails([FromBody] BookingDetails bookingDetails)
        {
            string flag;
            try
            {
                List<ResponseStatus> result = _userService.SaveBookingDetails(bookingDetails);

                if (!result.Any())
                {
                    return NotFound("{\"status\": \"Failed\"}");
                }
                flag = result[0].Error_desc;
                if (flag.Contains("Success"))
                {
                    return Ok(result);
                }

                return NotFound(result);

            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Insertion Failed\"}");
            }
        }

        [HttpGet("GetDriverNotificationDetails")]
        public IActionResult GetDriverNotificationDetails()
        {
            try
            {
                List<DriverNotification> result = _userService.GetDriverNotificationDetails();
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if (result.Count == 0)
                {
                    return NotFound("{\"status\": \"No Data Found\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpPost("PostDriverPaymentDetails")]
        public async Task<IActionResult> DriverPaymentDetails([FromBody] DriverPaymentDetails driverPaymentDetails)
        {
            string flag;
            try
            {
                List<ResponseStatus> result = await _userService.DriverPaymentDetails(driverPaymentDetails);

                if (!result.Any())
                {
                    return NotFound("{\"status\": \"Failed\"}");
                }
                flag = result[0].Error_desc;
                if (flag.Contains("Success"))
                {
                    return Ok(result);
                }

                return NotFound(result);

            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Insertion Failed\"}");
            }
        }

        [HttpGet("GetDriverPaymentDetails")]
        public IActionResult GetDriverPaymentDetails()
        {
            try
            {
                List<GetDriverPaymentDetails> result = _userService.GetDriverPaymentDetails();
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if (result.Count == 0)
                {
                    return NotFound("{\"status\": \"No Data Found\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }
    }
}

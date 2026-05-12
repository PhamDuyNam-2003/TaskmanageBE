using System;
using System.Collections.Generic;
using System.Text;

namespace BE.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; }
          = string.Empty;
    }
}

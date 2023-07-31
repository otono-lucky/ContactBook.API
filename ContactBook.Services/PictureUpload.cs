using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Services
{
    public class PictureUpload
    {
        private readonly Cloudinary _cloudinary;
        public PictureUpload(IConfiguration config)
        {
            Account account = new Account
            {
                Cloud = config.GetSection("CloudinarySettings: CloudName").Value,
                ApiKey = config.GetSection("CloudinarySettings: Apikey").Value,
                ApiSecret = config.GetSection("CloudinarySettings: ApiSecret").Value
            };
            _cloudinary = new Cloudinary(account);
        }

        public Cloudinary GetPicture()
        {
            return _cloudinary;
        }
    }
}

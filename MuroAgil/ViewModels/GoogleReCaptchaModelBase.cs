using Microsoft.AspNetCore.Mvc;
using MuroAgil.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace MuroAgil.Models {
    public abstract class GoogleReCaptchaModelBase {
        [Required(ErrorMessage = "Debe validar que no es un robot.")]
        [GoogleReCaptchaValidation]
        [BindProperty(Name = "g-recaptcha-response")]
        public String GoogleReCaptchaResponse { get; set; }
    }
}

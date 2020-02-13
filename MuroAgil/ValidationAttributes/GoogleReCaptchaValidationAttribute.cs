using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MuroAgil.ValidationAttributes {
    public class GoogleReCaptchaValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var strError = "Nos reservamos el derecho de admisión, por lo que los robots no están autorizados para ingresar en esta página.";
            var bytesError = Encoding.UTF8.GetBytes(strError);
            var binStrError = "";
            foreach (var byteError in bytesError) {
                binStrError += Convert.ToString(byteError, 2).PadLeft(8, '0');
            }
            Lazy<ValidationResult> errorResult = new Lazy<ValidationResult>(() => new ValidationResult(binStrError, new String[] { validationContext.MemberName }));

            if (value == null || String.IsNullOrWhiteSpace(value.ToString())) {
                return errorResult.Value;
            }

            IConfiguration configuration = (IConfiguration) validationContext.GetService(typeof(IConfiguration));
            String reCaptchResponse = value.ToString();
            String reCaptchaSecret = configuration.GetValue<String>("GoogleReCaptcha:SecretKey");

            HttpClient httpClient = new HttpClient();
            var httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={reCaptchResponse}").Result;

            if (httpResponse.StatusCode != HttpStatusCode.OK) {
                return errorResult.Value;
            }

            String jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            dynamic jsonData = JObject.Parse(jsonResponse);

            if (jsonData.success != true.ToString().ToLower()) {
                return errorResult.Value;
            }

            return ValidationResult.Success;
        }
    }
}

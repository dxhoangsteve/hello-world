using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BaseSource.ViewModels.Helper
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object[] _isValues;

        public RequiredIfAttribute(string propertyName, params object[] isValues)
        {
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            _isValues = isValues ?? throw new ArgumentNullException(nameof(isValues));
        }

        public override string FormatErrorMessage(string name)
        {
            //var errorMessage = $"Property {name} is required when {_propertyName} is {_isValue}";
            var errorMessage = string.Format("{0} không được để trống", name);
            return ErrorMessage ?? errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ArgumentNullException.ThrowIfNull(validationContext);
            var property = validationContext.ObjectType.GetProperty(_propertyName);

            if (property == null)
            {
                throw new NotSupportedException($"Can't find {_propertyName} on searched type: {validationContext.ObjectType.Name}");
            }

            var requiredIfTypeActualValue = property.GetValue(validationContext.ObjectInstance);

            //if (requiredIfTypeActualValue == null && _isValue != null)
            //{
            //    return ValidationResult.Success;
            //}

            //if (requiredIfTypeActualValue == null || requiredIfTypeActualValue.Equals(_isValue))
            
            if (_isValues.Contains(requiredIfTypeActualValue))
            {
                return value == null
                    ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
                    : ValidationResult.Success;
            }

            return ValidationResult.Success;
        }
    }
}

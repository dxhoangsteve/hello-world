using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Utilities.Extensions
{
    public class JsonModelBinder : IModelBinder
    {
        //public Task BindModelAsync(ModelBindingContext bindingContext)
        //{
        //    if (bindingContext == null)
        //    {
        //        throw new ArgumentNullException(nameof(bindingContext));
        //    }

        //    // Check the value sent in
        //    var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        //    if (valueProviderResult != ValueProviderResult.None)
        //    {
        //        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        //        // Attempt to convert the input value
        //        var valueAsString = valueProviderResult.FirstValue;
        //        var result = Newtonsoft.Json.JsonConvert.DeserializeObject(valueAsString, bindingContext.ModelType);
        //        if (result != null)
        //        {
        //            bindingContext.Result = ModelBindingResult.Success(result);
        //            return Task.CompletedTask;
        //        }
        //    }

        //    return Task.CompletedTask;
        //}

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            // Fetch the value of the argument by name and set it to the model state
            string fieldName = bindingContext.FieldName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(fieldName);
            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;
            else bindingContext.ModelState.SetModelValue(fieldName, valueProviderResult);

            // Do nothing if the value is null or empty
            string value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value)) return Task.CompletedTask;

            try
            {
                // Deserialize the provided value and set the binding result
                object result = JsonConvert.DeserializeObject(value, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonException)
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}

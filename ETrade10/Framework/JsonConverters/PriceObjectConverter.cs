﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OkonkwoETrade10.Framework.Factories;

namespace OkonkwoETrade10.Framework.JsonConverters
{
   public class PriceObjectConverter : JsonConverterBase
   {
      public override bool CanConvert(Type objectType)
      {
         return objectType == typeof(IHasPrices);
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var jo = new JObject();
         var type = value.GetType();

         var priceObject = value as IHasPrices;
         var priceProperties = priceObject.priceInformation.priceProperties;
         //var pricePrecision = Math.Abs(priceObject.priceInformation.instrument.displayPrecision);

         foreach (var property in type.GetRuntimeProperties())
         {
            if (property.CanRead)
            {
               object propertyValue = property.GetValue(value, null);

               if (propertyValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                  continue;
               if (property.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(JsonIgnoreAttribute)) != null)
                  continue;

               if (priceProperties.Contains(property.Name))
               {
                  decimal price = Convert.ToDecimal(propertyValue ?? 0);
                  //string formattedPrice = price.ToString("F" + pricePrecision, CultureInfo.InvariantCulture);

                  //jo.Add(property.Name, JToken.FromObject(formattedPrice));
               }
               else
                  jo.Add(property.Name, JToken.FromObject(propertyValue, serializer));
            }
         }

         jo.WriteTo(writer);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         var jsonToken = JToken.Load(reader);

         if (jsonToken.Type == JTokenType.Array)
         {
            var priceObjects = new List<IHasPrices>();

            var jsonArray = (JArray)jsonToken;

            foreach (var item in jsonArray)
            {
               var priceObject = PriceObjectFactory.Create(objectType.FullName);
               serializer.Populate(item.CreateReader(), priceObject);
               priceObjects.Add(priceObject);
            }

            return priceObjects;
         }
         else if (jsonToken.Type == JTokenType.Object)
         {
            var priceObject = PriceObjectFactory.Create(objectType.FullName);
            serializer.Populate(jsonToken.CreateReader(), priceObject);
            return priceObject;
         }
         else
            throw new ArgumentException(string.Format("Unexpected JTokenType ({0}) in reader.", jsonToken.Type.ToString()));
      }
   }
}

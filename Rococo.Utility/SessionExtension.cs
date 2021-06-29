using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rococo.Utility
{
    // In ASP.NET Core default session only store int value
    // So here inplement extension method to store and retreive any value such as object, list , int, string 
    // or anything.

    public static class SessionExtension
    {
        /// <summary>
        /// Store any value to session ex: object, list, int, string, byte etc
        /// </summary>
        /// <param name="session">Current Session</param>
        /// <param name="key">Session key which correspond the value will be set</param>
        /// <param name="value">Session value that need to store</param>
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }


        /// <summary>
        /// Get any type of session value.
        /// </summary>
        /// <typeparam name="T">typeof retreive data</typeparam>
        /// <param name="session">Current session</param>
        /// <param name="key">The key which correspond the value was saved</param>
        /// <returns></returns>
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

    }
}

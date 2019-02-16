
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Jekoss.Implementations.Helpers;
using JekossTest.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jekoss.Implementations.Common
{
    public class BaseService<TService>
    {
        public ILogger<TService> Logger;
        public SettingConfigurations Configurations;
        private IMemoryCache _cache;
        public BaseService (IConfigurationRoot root, ILogger<TService> logger, IMemoryCache memoryCache)
        {
            Logger = logger;
            Configurations = new SettingConfigurations(root);
            _cache = memoryCache;
        }
        
        protected async Task<BaseResponse> ProcessRequestAsync(Func<Task<BaseResponse>> func)
        {
            try
            {
                var response = await func();
                return response;
            }
            catch (Exception ex)
            {
                HandleError(ex, func.Target);
                return new BaseResponse(false, ex.Message);
            }
        }
        
        
        protected async Task<BaseResponse<TModel>> ProcessRequestAsync<TModel>(Func<Task<BaseResponse<TModel>>> func)
        {
            try
            {
                var responce = await func();
                return responce;
            }
            catch (Exception ex)
            {
                HandleError(ex, func.Target);
                return new BaseResponse<TModel>(false,ex.InnerException.ToString(),default(TModel));
            }
        }

        protected async Task<BaseResponse<TModel>> ProcessRequestCacheAsync<TModel>(
            Func<Task<BaseResponse<TModel>>> func, object keyObj, TimeSpan expiration)
        {
            try
            {
                var key = BuildUniqueCacheKey(func.Method.Name, keyObj);
#if !DEBUG
                var model = GetFromCache<TModel>(key);
                if (!EqualityComparer<TModel>.Default.Equals(model, default(TModel)))
                {
                    return new BaseResponse<TModel>(model);
                }
#endif
                var response = await func();
                if (response.Success)
                {
                    AddToCache(key, expiration, response.Model);
                }
                return response;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        
        private void HandleError(Exception ex, object request)
        {
            Logger.LogError(ex.ToString());
        }

        private void AddToCache<TModel>(string key, TimeSpan expiration, TModel model)
        {
            _cache.Set(key,model,expiration);
        }
        
        private TModel GetFromCache<TModel>(string key)
        {
            return _cache.Get<TModel>(key); 
        }
        
        
        private static string BuildUniqueCacheKey(string name, object parameters)
        {
            return $"{name}?{ConvertObjectToUniqueString(parameters)}";
        }
        
        private static string ConvertObjectToUniqueString(object obj)
        {
            var objType = obj.GetType();

            if (objType.IsValueType || objType == typeof(string) || objType == typeof(DateTime))
            {
                return obj.ToString();
            }

            var props = new List<PropertyInfo>(objType.GetProperties());
            var result = new List<string>();
            foreach (var property in props)
            {
                result.Add(property.Name + "=" + property.GetValue(obj));
            }
            return string.Join("&", result);
        }

    }
}
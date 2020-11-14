using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App
{
    public class RequestMiddleware 
    { 
        private readonly RequestDelegate _next; 
 
        public RequestMiddleware(RequestDelegate next) 
        { 
            _next = next; 
        } 
        
        public async Task Invoke(HttpContext context)
        { 
            var rand = new Random();
            if (rand.NextDouble() < 0.5)
            {
                Console.WriteLine("[RequestMiddleware] Normal run");
                await _next(context);    
            }
            else
            {
                Console.WriteLine("[RequestMiddleware] Throw");
                throw new Exception("wololo");
            }
            
        } 
}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace SubscribeDb
{
    public static class UseSubscribeDbExtensions
    {
        private static string ToDataRecord<T>(T o)
        {
            return JsonConvert.SerializeObject(o);
        }

        private static T FromDataRecord<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }

        public static void SubscribeDb<T>(this IApplicationBuilder app)
        {
            var routeBuilder = new RouteBuilder(app);

            string path = $"{typeof(T).Name}/{{id}}";

            routeBuilder.MapGet(path, async context =>
            {
                string id = context.GetRouteValue("id") as string;
                if (context.Request.Query["watch"].Any())
                {
                    context.Response.ContentType = "text/event-stream";

                    foreach ( var change in Database.Watch(id) )
                    {
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(change));
                        await context.Response.WriteAsync(Environment.NewLine);
                    }
                }
                else 
                {
                    // TODO: Not Found
                    T value = FromDataRecord<T>(Database.Get(id));
                    
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(value));
                }
            });

            routeBuilder.MapPost(path, context => {

                string id = context.GetRouteValue("id") as string;

                string body = null;
                using ( var reader = new StreamReader(context.Request.Body))
                {
                    body = reader.ReadToEnd();
                }

                T data = JsonConvert.DeserializeObject<T>(body);

                Database.Put(id, ToDataRecord(data));

                return context.Response.WriteAsync("ok");
            });

            routeBuilder.MapDelete(path, async context =>
            {
                string id = context.GetRouteValue("id") as string;

                // TODO: Not Found
                T value = FromDataRecord<T>(Database.Delete(id));
                
                await context.Response.WriteAsync(JsonConvert.SerializeObject(value));
            });

            // TODO: Correct Put/Post behaviour

            var routes = routeBuilder.Build();
            app.UseRouter(routes);
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;

namespace EventBudDemo
{
    public class PollyDemo
    {
        public void PollyTest()
        {
            //重试策略
            var polly0 = Policy<HttpResponseMessage>.Handle<Exception>()
               .WaitAndRetry(5, a => TimeSpan.FromMinutes(Math.Pow(2, a)), (ex, time) =>
               {
                   Console.WriteLine(ex.Result);
               });
            //降级处理 一般返回更友好的消息
            var polly1 = Policy<HttpResponseMessage>.HandleInner<Exception>()
                .Fallback(new HttpResponseMessage(), async b =>
                {
                    Console.WriteLine($"{b.Result.Content}");
                });
            //熔断处理
            var polly2 = Policy.Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(5), onBreak:(ex, time) =>
                {
                    Console.WriteLine(ex.Message);
                },
                onReset:  () =>
                    {
                        Console.WriteLine();
                    },
                onHalfOpen: () =>
                {
                    Console.WriteLine();
                }
                  );
        }
    }
}

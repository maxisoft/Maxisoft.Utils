using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Maxisoft.Utils.Process;
using Xunit;

namespace Maxisoft.Utils.Tests.Process
{
    public class ProcessExtentionTest
    {
        [Fact]
        public async Task TestWaitForExitAsync_Nominal()
        {
            var process = StartPingProcess(1);
            await process.WaitForExitAsync().ToObservable().Timeout(TimeSpan.FromSeconds(5));
            Assert.True(process.HasExited);
        }

        [Fact]
        public async Task TestWaitForExitAsync_WithToken()
        {
            var process = StartPingProcess(50);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(1));
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await process.WaitForExitAsync(cts.Token)
                .ToObservable().Timeout(TimeSpan.FromSeconds(5)));
            Assert.False(process.HasExited);
            process.Kill();
            cts = new CancellationTokenSource();
            await process.WaitForExitAsync(cts.Token).ToObservable().Timeout(TimeSpan.FromSeconds(5));
            Assert.True(process.HasExited);
        }

        private static System.Diagnostics.Process StartPingProcess(int count)
        {
            var info = new ProcessStartInfo();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info.FileName = "cmd";
                info.Arguments = $"/c \"ping 127.0.0.1 -n {count}\"";
            }
            else //assume unix
            {
                info.FileName = "sh";
                info.Arguments = "-c \"ping -c {count} 127.0.0.1\"";
            }
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            var process = System.Diagnostics.Process.Start(info);
            return process;
        }
    }
}
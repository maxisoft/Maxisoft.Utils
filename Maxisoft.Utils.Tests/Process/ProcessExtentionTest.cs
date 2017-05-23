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
            ProcessStartInfo info = new ProcessStartInfo();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info.FileName = "cmd";
                info.Arguments = "/c \"ping 127.0.0.1 -n 2\"";
            }
            else //assume unix
            {
                info.FileName = "sh";
                info.Arguments = "-c \"ping -c 2 127.0.0.1\"";
            }
            var process = System.Diagnostics.Process.Start(info);
            await process.WaitForExitAsync().ToObservable().Timeout(TimeSpan.FromSeconds(5));
            Assert.True(process.HasExited);
        }

        [Fact]
        public async Task TestWaitForExitAsync_WithToken()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info.FileName = "cmd";
                info.Arguments = "/c \"ping 127.0.0.1 -n 50\"";
            }
            else //assume unix
            {
                info.FileName = "sh";
                info.Arguments = "-c \"ping -c 50 127.0.0.1\"";
            }
            var cts = new CancellationTokenSource();
            var process = System.Diagnostics.Process.Start(info);
            cts.CancelAfter(TimeSpan.FromSeconds(1));
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await process.WaitForExitAsync(cts.Token)
                .ToObservable().Timeout(TimeSpan.FromSeconds(5)));
            Assert.False(process.HasExited);
            process.Kill();
            cts = new CancellationTokenSource();
            await process.WaitForExitAsync(cts.Token).ToObservable().Timeout(TimeSpan.FromSeconds(5));
            Assert.True(process.HasExited);
        }
    }
}
namespace TechBlog.Domain.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static CancellationToken GenerateCancellationToken(this CancellationTokenSource cancellationTokenSource, int hoursForCancellation, int minutesForCancellation, int secondsForCancellation)
        {
            cancellationTokenSource.CancelAfter(new TimeSpan(hoursForCancellation, minutesForCancellation, secondsForCancellation));

            return cancellationTokenSource.Token;
        }
    }
}

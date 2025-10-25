using Nito.AsyncEx;

namespace SecondTask
{
    public class AsyncCounterService
    {
        private static int count;
        private static AsyncReaderWriterLock _lock;

        static AsyncCounterService()
        {
            count = 0;
            _lock = new AsyncReaderWriterLock();
        }

        public static async Task AddToCount(int value)
        {
            using (await _lock.WriterLockAsync())
            {
                await DelaySimulation();
                count += value;
            }
        }

        public static async Task<int> GetCount()
        {
            using (await _lock.ReaderLockAsync())
            {
                await DelaySimulation();
                return count;
            }

        }

        // для имитации задержек - важно для тестов 
        private static Task DelaySimulation()
        {
            var r = new Random();

            return Task.Delay(r.Next(1, 100));
        }
    }
}

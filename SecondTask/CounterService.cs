namespace SecondTask
{
    public static class CounterService
    {

        private static int count;
        private static ReaderWriterLockSlim _lock;

        static CounterService()
        {
            count = 0;
            _lock = new ReaderWriterLockSlim();
        }

        // можно и без Task как void, но в случае интеграции куда то могут быть проблемы с производительностью из-за ожидания очереди
        public static Task AddToCount(int value)
        {
            try
            {
                _lock.EnterWriteLock();

                count += value;
            }
            finally
            {
                // обязательно освобождаем
                _lock.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        public static Task<int> GetCount()
        {
            try
            {
                _lock.EnterReadLock();

                return Task.FromResult(count);
            }
            finally
            {
                // обязательно освобождаем
                _lock.ExitReadLock();
            }
        }
    }
}

namespace SecondTask.Tests
{
    public class CounterServiceTests
    {
        // Сброс статического состояния перед каждым тестом
        public CounterServiceTests()
        {
            ResetCounterService();
        }

        private static void ResetCounterService()
        {
            var type = typeof(CounterService);
            var countField = type.GetField("count",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var lockField = type.GetField("_lock",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            countField?.SetValue(null, 0);
            var oldLock = lockField?.GetValue(null) as ReaderWriterLockSlim;
            oldLock?.Dispose();
            lockField?.SetValue(null, new ReaderWriterLockSlim());
        }

        [Fact]
        public async Task GetCount_ReturnsZero_Initially()
        {
            var count = await CounterService.GetCount();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task AddToCount_IncrementsCount()
        {
            await CounterService.AddToCount(5);
            var count = await CounterService.GetCount();
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task MultipleAdds_AreAccumulated()
        {
            await CounterService.AddToCount(10);
            await CounterService.AddToCount(-3);
            await CounterService.AddToCount(7);

            var count = await CounterService.GetCount();
            Assert.Equal(14, count);
        }

        [Fact]
        public async Task ConcurrentReaders_CanReadSimultaneously_AndReturnConsistentValue()
        {
            int initialValue = 42;
            int readerCount = 100;

            await CounterService.AddToCount(initialValue);
             
            var tasks = new Task<int>[readerCount];
            for (int i = 0; i < readerCount; i++)
            {
                tasks[i] = CounterService.GetCount();
            }

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                Assert.Equal(initialValue, result);
            }
        }

        [Fact]
        public async Task ConcurrentWriters_ExecuteSequentially_WithoutRaceCondition()
        {
            int iterations = 1000;
            var tasks = new Task[iterations];
            for (int i = 0; i < iterations; i++)
            {
                tasks[i] = CounterService.AddToCount(1);
            }

            await Task.WhenAll(tasks);

            var finalCount = await CounterService.GetCount();
            Assert.Equal(iterations, finalCount);
        }

        [Fact]
        public async Task MixedReadersAndWriters_ProduceConsistentResults()
        {
            int writerCount = 100;
            int readerCount = 100;

            var tasks = new List<Task>();

            // Запускаем читателей и писателей параллельно
            for (int i = 0; i < writerCount; i++)
            {
                tasks.Add(CounterService.AddToCount(1));
            }

            var readerResults = new int[readerCount];
            for (int i = 0; i < readerCount; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    readerResults[index] = await CounterService.GetCount();
                }));
            }

            await Task.WhenAll(tasks);

            var finalCount = await CounterService.GetCount();
            Assert.Equal(writerCount, finalCount);

            // Все промежуточные чтения должны быть в диапазоне [0, writerCount]
            foreach (var r in readerResults)
            {
                Assert.InRange(r, 0, writerCount);
            }
        }
    }
}

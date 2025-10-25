using System.Text;

namespace FirstTask
{
    public static class StringCompressionUtility
    {

        private readonly static HashSet<char> digits = new HashSet<char>()
            {'0','1','2','3', '4', '5', '6', '7', '8', '9'};

        public static string Compress(string input)
        {
            //проверка невалидных значений
            if (input == null)
                return null;
            if (input == string.Empty)
                return string.Empty;

            // если использовать простой string может быть много алокаций на длинных строках
            StringBuilder output = new StringBuilder();

            //оптимальным будет использовать Span для прохода по строке - для уменьшения алокаций.
            var inputSpan = input.AsSpan();

            Console.WriteLine($"Длина строки {inputSpan.Length}");

            uint charCount = 1;

            for (int i = 0; i < inputSpan.Length - 1; i++)
            {
                //проверяем что слудующий знак равен текущему
                if (inputSpan[i].Equals(inputSpan[i + 1]))
                {
                    charCount++;
                    continue;
                }
                // если следующий знак не равен текущему то записываем в стринг билдер
                else if (charCount > 1)
                {
                    output.Append(inputSpan[i]);
                    output.Append(charCount);
                    charCount = 1;
                    continue;
                }
                else
                {
                    output.Append(inputSpan[i]);
                    continue;
                }
            }

            // Обработка последней группы символов
            // В этот момент charCount содержит количество повторений последнего символа подряд
            if (charCount > 1)
            {
                // Предполагается, что последний символ строки такой же, как и предпоследний
                output.Append(inputSpan[inputSpan.Length - 1]);
                output.Append(charCount);
            }
            else
            {
                // charCount == 1 для последнего символа
                output.Append(inputSpan[inputSpan.Length - 1]);
            }


            return output.ToString();

        }

        public static string Decompress(string input)
        {
            //проверка невалидных значений
            if (input == null)
                return null;
            if (input == string.Empty)
                return string.Empty;

            // если использовать простой string может быть много алокаций на длинных строках
            StringBuilder output = new StringBuilder();

            //оптимальным будет использовать Span для прохода по строке - для уменьшения алокаций
            var inputSpan = input.AsSpan();

            Console.WriteLine($"Длина строки {inputSpan.Length}");


            // условия задачи такие что число вначале строки быть не может
            int startDigit = 0;

            for (int i = 0; i < inputSpan.Length;)
            {
                // проверяем что знак цифра
                // проверка с хэш-сетом должна быть быстрее чем с простым array или List
                if (digits.Contains(inputSpan[i]))
                {
                    // В сжатом формате число всегда идет после буквы.
                    if (i == 0)
                    {
                        throw new FormatException("Invalid compressed string format: cannot start with a digit.");
                    }

                    startDigit = i;

                    // ищем где число кончается
                    while (i < inputSpan.Length && digits.Contains(inputSpan[i]))
                    {
                        i++;
                    }

                    var countSlice = inputSpan.Slice(startDigit, i - startDigit);

                    bool successParse = int.TryParse(countSlice, out int count);

                    if (successParse)
                    {
                        output.Append(inputSpan[startDigit - 1], count - 1);
                    }
                }
                else
                {
                    output.Append(inputSpan[i]);
                    i++;
                }
            }

            return output.ToString();
        }

    }
}

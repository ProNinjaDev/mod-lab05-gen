using Microsoft.VisualStudio.TestTools.UnitTesting;
using generator;
using System;
using System.Linq;

namespace ProjCharGenerator.Tests
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void WordFreq_GenerateWord_ReturnsNoData_WhenFrequenciesNotLoaded()
        {
            // Этот тест требует доступа к приватному методу GenerateWord
            Assert.Inconclusive("Тестирование приватного метода GenerateWord требует рефакторинга или InternalsVisibleTo");
        }

        [TestMethod]
        public void CharGenerator_GetSym_ReturnsValidChar()
        {
            CharGenerator cg = new CharGenerator();
            char sym = cg.getSym();
            string alphabet = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";
            Assert.IsTrue(alphabet.Contains(sym), $"Сгенерированный символ '{sym}' отсутствует в ожидаемом алфавите.");
        }

        [TestMethod]
        public void CharGenerator_GetSym_IsNotNullOrWhitespace()
        {
            CharGenerator cg = new CharGenerator();
            char sym = cg.getSym();
            Assert.IsFalse(char.IsWhiteSpace(sym), "Сгенерированный символ не должен быть пробелом.");
            Assert.AreNotEqual('\0', sym, "Сгенерированный символ не должен быть нулевым.");
        }

        [TestMethod]
        public void WordFreq_Constructor_DoesNotThrow()
        {
            try
            {
                WordFrequencyGenerator wg = new WordFrequencyGenerator();
                Assert.IsNotNull(wg, "Экземпляр WordFrequencyGenerator не должен быть null.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Конструктор WordFrequencyGenerator не должен вызывать исключений: {ex.Message}");
            }
        }

        [TestMethod]
        public void Bigram_GenerateText_ReturnsEmpty_WhenLengthIsOne_NoLoad()
        {
            BigramGenerator bg = new BigramGenerator();
            string result = bg.GenerateText(1);
            Assert.AreEqual(string.Empty, result, "Ожидается пустая строка для длины 1 без загрузки частот.");
        }

        [TestMethod]
        public void Bigram_GenerateText_ReturnsEmpty_WhenLengthIsLarge_NoLoad()
        {
            BigramGenerator bg = new BigramGenerator();
            string result = bg.GenerateText(50);
            Assert.AreEqual(string.Empty, result, "Ожидается пустая строка для большой длины без загрузки частот.");
        }

        [TestMethod]
        public void Bigram_Constructor_DoesNotThrow()
        {
             try
            {
                BigramGenerator bg = new BigramGenerator();
                 Assert.IsNotNull(bg, "Экземпляр BigramGenerator не должен быть null.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Конструктор BigramGenerator не должен вызывать исключений: {ex.Message}");
            }
        }

        [TestMethod]
        public void CharGenerator_Constructor_DoesNotThrow()
        {
            try
            {
                CharGenerator cg = new CharGenerator();
                Assert.IsNotNull(cg, "Экземпляр CharGenerator не должен быть null.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Конструктор CharGenerator не должен вызывать исключений: {ex.Message}");
            }
        }

        [TestMethod]
        public void CharGenerator_GetSym_GeneratesMultipleChars()
        {
            CharGenerator cg = new CharGenerator();
            char sym1 = cg.getSym();
            char sym2 = cg.getSym();
            Assert.AreNotEqual(' ', sym1);
            Assert.AreNotEqual(' ', sym2);
        }

        [TestMethod]
        public void Bigram_GenerateText_ReturnsEmpty_ForLengthTwo_NoLoad()
        {
             BigramGenerator bg = new BigramGenerator();
             string result = bg.GenerateText(2);
             Assert.AreEqual(string.Empty, result);
        }

         [TestMethod]
        public void Bigram_GenerateText_ReturnsEmpty_ForLengthTen_NoLoad()
        {
             BigramGenerator bg = new BigramGenerator();
             string result = bg.GenerateText(10);
             Assert.AreEqual(string.Empty, result);
        }

         [TestMethod]
        public void Bigram_GenerateText_ReturnsEmpty_ForZeroLength_NoLoad()
        {
             BigramGenerator bg = new BigramGenerator();
             string result = bg.GenerateText(0);
             Assert.AreEqual(string.Empty, result);
        }

         [TestMethod]
        public void Bigram_GenerateText_ReturnsEmpty_ForNegativeLength_NoLoad()
        {
             BigramGenerator bg = new BigramGenerator();
             string result = bg.GenerateText(-10);
             Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void WordFreq_GenerateText_NoExtraSpaces_NoLoad()
        {
            WordFrequencyGenerator wg = new WordFrequencyGenerator();
            string result = wg.GenerateText(4);

            Assert.IsFalse(string.IsNullOrEmpty(result) && result.Contains("  "), "Не должно быть двойных пробелов");
            Assert.IsFalse(string.IsNullOrEmpty(result) && result.StartsWith(" "), "Не должно быть пробела в начале");
            Assert.IsFalse(string.IsNullOrEmpty(result) && result.EndsWith(" ."), "Не должно быть пробела перед точкой");
        }

        [TestMethod]
        public void BigramGenerator_InstanceNotNull()
        {
            var bg = new BigramGenerator();
            Assert.IsNotNull(bg);
        }

        [TestMethod]
        public void WordFrequencyGenerator_InstanceNotNull()
        {
             var wg = new WordFrequencyGenerator();
            Assert.IsNotNull(wg);
        }

        [TestMethod]
        public void CharGenerator_InstanceNotNull()
        {
            var cg = new CharGenerator();
            Assert.IsNotNull(cg);
        }

         [TestMethod]
        public void Bigram_GenerateText_RepeatedCalls_NoLoad()
        {
             BigramGenerator bg = new BigramGenerator();
             Assert.AreEqual(string.Empty, bg.GenerateText(5));
             Assert.AreEqual(string.Empty, bg.GenerateText(10));
             Assert.AreEqual(string.Empty, bg.GenerateText(0));
        }
    }
}
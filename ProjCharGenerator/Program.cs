using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using ScottPlot;
using System.Linq;

namespace generator
{
    public class CharGenerator
    {
        private string syms = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя"; 
        private char[] data;
        private int size;
        private Random random = new Random();
        public CharGenerator() 
        {
           size = syms.Length;
           data = syms.ToCharArray(); 
        }
        public char getSym() 
        {
           return data[random.Next(0, size)]; 
        }
    }

    public class BigramGenerator
    {
        private readonly Dictionary<char, Dictionary<char, int>> frequencies = new Dictionary<char, Dictionary<char, int>>();
        private readonly string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя"; 
        private readonly Random randomInst = new Random();
        private readonly string bigramFileName = "bigrams.txt";

        public void LoadFrequencies()
        {
            frequencies.Clear(); 

            if (!File.Exists(bigramFileName))
            {
                return; 
            }

            string[] lines = File.ReadAllLines(bigramFileName);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && parts[0].Length == 2 && int.TryParse(parts[1], out int frequency))
                {
                    char firstChar = char.ToLower(parts[0][0]);
                    char secondChar = char.ToLower(parts[0][1]);

                    if (alphabet.Contains(firstChar) && alphabet.Contains(secondChar) && frequency > 0) 
                    {
                       if (!frequencies.ContainsKey(firstChar))
                       {
                           frequencies[firstChar] = new Dictionary<char, int>();
                       }
                       frequencies[firstChar][secondChar] = frequency;
                    }
                }
            }
        }

        private char GetNextChar(char previousChar)
        {
            previousChar = char.ToLower(previousChar);

            if (!frequencies.ContainsKey(previousChar) || frequencies[previousChar].Count == 0)
            {
                return alphabet[randomInst.Next(alphabet.Length)];
            }

            Dictionary<char, int> possibleNext = frequencies[previousChar];
            
            int totalWeight = 0;
            foreach (var pair in possibleNext) 
            {
                if (alphabet.Contains(pair.Key)) 
                {
                    totalWeight += pair.Value;
                }
            }
            
            if (totalWeight <= 0)
            {
                return alphabet[randomInst.Next(alphabet.Length)];
            }

            int randomValue = randomInst.Next(1, totalWeight + 1);
            int currentSum = 0;

            foreach (var pair in possibleNext)
            {
                if (!alphabet.Contains(pair.Key)) continue;

                currentSum += pair.Value;
                if (randomValue <= currentSum)
                {
                    return pair.Key;
                }
            }

            List<char> fallbackKeys = new List<char>();
            foreach(char key in possibleNext.Keys)
            {
                if(alphabet.Contains(key)) fallbackKeys.Add(key);
            }
            if(fallbackKeys.Count > 0)
                 return fallbackKeys[randomInst.Next(fallbackKeys.Count)]; 
            else
                 return alphabet[randomInst.Next(alphabet.Length)]; 
        }

        public string GenerateText(int length)
        {
             if (frequencies.Count == 0) 
             {
                 return string.Empty; 
             }
             if (length <= 0) return string.Empty;

            StringBuilder generatedText = new StringBuilder(length);

            List<char> startingChars = new List<char>();
            foreach(char c in frequencies.Keys)
            {
                int weightSum = 0;
                if (frequencies.ContainsKey(c))
                {
                   Dictionary<char, int> innerDict = frequencies[c];
                   foreach (var kvp in innerDict)
                   {
                       if (alphabet.Contains(kvp.Key))
                       {
                           weightSum += kvp.Value;
                       }
                   }
                }
                if (weightSum > 0) startingChars.Add(c);
            }

            if (startingChars.Count == 0)
            {
                return string.Empty; 
            }
            
            char currentChar = startingChars[randomInst.Next(startingChars.Count)];
            generatedText.Append(currentChar);

            for (int i = 1; i < length; i++)
            {
                currentChar = GetNextChar(currentChar); 
                generatedText.Append(currentChar);
            }

            return generatedText.ToString();
        }
    }

    public class WordFrequencyGenerator
    {
        private readonly List<string> _words = new List<string>();
        private readonly List<double> _cumulativeFrequencies = new List<double>();
        private double _totalFrequencySum = 0;
        private readonly Random _randomInst = new Random();
        private readonly string _wordFrequencyFileName = "word_frequencies.txt";

        public void LoadFrequencies()
        {
            _words.Clear();
            _cumulativeFrequencies.Clear();
            _totalFrequencySum = 0;

            if (!File.Exists(_wordFrequencyFileName))
            {
                return; 
            }

            string[] lines = File.ReadAllLines(_wordFrequencyFileName);
            double currentCumulativeSum = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                 string[] parts = line.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries); 
                if (parts.Length == 2 && double.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double frequency))
                {
                    if (frequency > 0)
                    {
                        string word = parts[0];
                        _words.Add(word);
                        currentCumulativeSum += frequency;
                        _cumulativeFrequencies.Add(currentCumulativeSum);
                    }
                }
            }
            _totalFrequencySum = currentCumulativeSum;
        }
        
        private string GenerateWord()
        {
             if (_totalFrequencySum <= 0 || _words.Count == 0)
             {
                 return "(no_data)";
             }

            double randomValue = _randomInst.NextDouble() * _totalFrequencySum;

             for (int i = 0; i < _cumulativeFrequencies.Count; i++)
             {
                 if (randomValue < _cumulativeFrequencies[i])
                 {
                     return _words[i];
                 }
             }

             return _words[_words.Count - 1]; 
        }

        public string GenerateText(int wordCount)
        {
            if (_words.Count == 0)
            {
                return string.Empty;
            }
            if (wordCount <= 0) return string.Empty;

            StringBuilder generatedText = new StringBuilder();
            for (int i = 0; i < wordCount; i++)
            {
                generatedText.Append(GenerateWord());
                if (i < wordCount - 1)
                {
                    generatedText.Append(" ");
                }
            }

             generatedText.Append("."); 

            return generatedText.ToString();
        }
    }

    class Program
    {
        private static readonly string alphabetForStats = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        
        private static readonly Dictionary<char, double> expectedFrequencies = new Dictionary<char, double>() {
            {'о', 0.090}, {'е', 0.072}, {'а', 0.062}, {'и', 0.062}, 
            {'т', 0.053}, {'н', 0.053}, {'с', 0.045}, {'р', 0.040}, 
            {'в', 0.038}, {'л', 0.035}, {'к', 0.028}, {'м', 0.026}, 
            {'д', 0.025}, {'п', 0.023}, {'у', 0.021}, {'я', 0.018}, 
            {'ы', 0.016}, {'з', 0.016}, {'ь', 0.014}, {'б', 0.014}, 
            {'г', 0.013}, {'ч', 0.012}, {'й', 0.010}, {'х', 0.009}, 
            {'ж', 0.007}, {'ю', 0.006}, {'ш', 0.006}, {'ц', 0.004}, 
            {'щ', 0.003}, {'э', 0.003}, {'ф', 0.002}
        };

        static void Main(string[] args)
        {
            BigramGenerator bigramGenerator = new BigramGenerator();
            bigramGenerator.LoadFrequencies(); 
            int bigramTextLength = 1001; 
            string bigramGeneratedText = bigramGenerator.GenerateText(bigramTextLength);

            if (!string.IsNullOrEmpty(bigramGeneratedText))
            {
                SaveTextToFile(bigramGeneratedText, "gen-1.txt");
                 PlotBigramStats(bigramGeneratedText, expectedFrequencies, "gen-1.png");

            }
             
             WordFrequencyGenerator wordGenerator = new WordFrequencyGenerator();
             wordGenerator.LoadFrequencies();
             int wordCount = 1000; 
             string wordGeneratedText = wordGenerator.GenerateText(wordCount);

            if (!string.IsNullOrEmpty(wordGeneratedText))
            {
                SaveTextToFile(wordGeneratedText, "gen-2.txt");
                 PlotWordStats(wordGeneratedText, "gen-2.png"); 
            }

        }
        
         private static void SaveTextToFile(string text, string filename)
         {
            string outputDirectory = Path.Combine("..", "Results");
            string outputFilePath = Path.Combine(outputDirectory, filename);
            Directory.CreateDirectory(outputDirectory);
            File.WriteAllText(outputFilePath, text, Encoding.UTF8);
         }
         
         private static void PlotBigramStats(string text, Dictionary<char, double> expectedFreqs, string filename)
         {
             SortedDictionary<char, int> stat = new SortedDictionary<char, int>();
             string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя"; 
            
             foreach (char ch in text)
             {
                  if (alphabet.Contains(ch))
                  {
                     if (stat.ContainsKey(ch))
                         stat[ch]++;
                     else
                         stat.Add(ch, 1);
                 }
             }

             if (stat.Count > 0)
             {
                 List<string> labels = new List<string>();
                 List<double> actualValues = new List<double>();
                 List<double> expectedValues = new List<double>();
                 
                 double totalCharsInStat = 0; 
                 foreach(int count in stat.Values) totalCharsInStat += count;

                 foreach (var entry in stat)
                 {
                     char character = entry.Key;
                     labels.Add(character.ToString());
                     actualValues.Add((double)entry.Value / totalCharsInStat);
                     expectedValues.Add(expectedFreqs.ContainsKey(character) ? expectedFreqs[character] : 0.0);
                 }

                 if (labels.Count > 0)
                 {
                     var plt = new ScottPlot.Plot(); 
                     double[] positions = ScottPlot.Generate.Consecutive(labels.Count);
                   
                     var barExpectedPlot = plt.Add.Bars(positions, expectedValues.ToArray());
                     barExpectedPlot.Label = "Expected"; 
                     barExpectedPlot.Color = Colors.Gray;

                     var barActualPlot = plt.Add.Bars(positions, actualValues.ToArray());
                     barActualPlot.Label = "Actual"; 
                     barActualPlot.Color = Colors.Purple;

                     plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                         positions.Select((p, i) => new ScottPlot.Tick(p, labels[i])).ToArray());
                     plt.Axes.Bottom.TickLabelStyle.Rotation = 0; 
                     plt.Axes.Bottom.Label.Text = "Character"; 
                     plt.Axes.Left.Label.Text = "Frequency"; 
                     plt.Title("Frequency Distribution for Bigrams"); 
                     plt.Legend.IsVisible = true;
                     plt.Legend.Location = Alignment.UpperRight;

                     string outputDirectory = Path.Combine("..", "Results");
                     string plotFilePath = Path.Combine(outputDirectory, filename);
                     try
                     { plt.SavePng(plotFilePath, 800, 500); } 
                     catch { } 
                 }
             }
         }

         private static void PlotWordStats(string text, string filename)
         {
             Dictionary<string, int> wordStat = new Dictionary<string, int>();
             string[] wordsInText = text.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
             double totalWordsInStat = wordsInText.Length;

             if (totalWordsInStat == 0) return;

             foreach (string word in wordsInText)
             {
                 string lowerWord = word.ToLowerInvariant();
                 if (wordStat.ContainsKey(lowerWord))
                     wordStat[lowerWord]++;
                 else
                     wordStat.Add(lowerWord, 1);
             }

             Dictionary<string, double> expectedWordFreqs = new Dictionary<string, double>();
             string wordFrequencyFileName = "word_frequencies.txt";
             double totalExpectedSum = 0;
             if (File.Exists(wordFrequencyFileName))
             {
                 string[] lines = File.ReadAllLines(wordFrequencyFileName);
                 foreach (string line in lines)
                 {
                     if (string.IsNullOrWhiteSpace(line)) continue;
                     string[] parts = line.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries); 
                     if (parts.Length == 2 && double.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double frequency))
                     {
                         if (frequency > 0)
                         {
                             string lowerWordFromFile = parts[0].ToLowerInvariant();
                             if (!expectedWordFreqs.ContainsKey(lowerWordFromFile))
                             {
                                 expectedWordFreqs.Add(lowerWordFromFile, frequency); 
                                 totalExpectedSum += frequency;
                             }
                         }
                     }
                 }
             }
             
             if (totalExpectedSum > 0)
             {
                 List<string> keys = new List<string>(expectedWordFreqs.Keys);
                 foreach (string key in keys)
                 {
                     expectedWordFreqs[key] = expectedWordFreqs[key] / totalExpectedSum;
                 }
             }

             List<string> labels = new List<string>();
             List<double> actualValues = new List<double>();
             List<double> expectedValues = new List<double>();

             var sortedWordStat = wordStat.OrderByDescending(pair => pair.Value);

             foreach (var entry in sortedWordStat)
             {
                 string wordLabel = entry.Key;
                 labels.Add(wordLabel);
                 actualValues.Add((double)entry.Value / totalWordsInStat);
                 expectedValues.Add(expectedWordFreqs.ContainsKey(wordLabel) ? expectedWordFreqs[wordLabel] : 0.0);
             }

             if (labels.Count > 0)
             {                 
                 var plt = new ScottPlot.Plot();
                 int plotWidth = Math.Max(800, labels.Count * 20);
                 int plotHeight = 600; 

                 double[] positions = ScottPlot.Generate.Consecutive(labels.Count);
                 
                 var barExpectedPlot = plt.Add.Bars(positions, expectedValues.ToArray());
                 barExpectedPlot.Label = "Expected (Normalized IPM)"; 
                 barExpectedPlot.Color = Colors.Gray;

                 var barActualPlot = plt.Add.Bars(positions, actualValues.ToArray());
                 barActualPlot.Label = "Actual"; 
                 barActualPlot.Color = Colors.DarkCyan;

                 plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                     positions.Select((p, i) => new ScottPlot.Tick(p, labels[i])).ToArray());
                 plt.Axes.Bottom.TickLabelStyle.Rotation = labels.Count > 15 ? 90 : 0; 
                 plt.Axes.Bottom.TickLabelStyle.FontSize = 8;

                 plt.Axes.Bottom.Label.Text = "Word"; 
                 plt.Axes.Left.Label.Text = "Frequency"; 
                 plt.Title("Word Frequency Distribution"); 
                 plt.Legend.IsVisible = true;
                 plt.Legend.Location = Alignment.UpperRight;

                 string outputDirectory = Path.Combine("..", "Results");
                 string plotFilePath = Path.Combine(outputDirectory, filename);
                 try
                 { plt.SavePng(plotFilePath, plotWidth, plotHeight); } 
                 catch { } 
             }
         }

    }
}


namespace one_brc;

using System.Diagnostics;

public record WeatherData
{
    public WeatherData(float firstValue)
    {
        Min = Max = Total = firstValue;
    }
    
    public int DataCount = 1;
    public float Min;
    public float Max;
    public float Total;
    public float Average => Total / DataCount;
}

public static class WeatherDataReader
{
    public static void ParseData(string path)
    {
        string line;
        var sw = new Stopwatch();
        var cities = new Dictionary<string, WeatherData>();
        var totalCount = 0;
        
        try
        {
            Console.WriteLine("Opening file...");
            sw.Start();    
            StreamReader sr = new StreamReader(path);

            line = sr.ReadLine();

            while (line != null)
            {
                totalCount++;

                if (totalCount % 1000000 == 0)
                    Console.WriteLine("Reading line " + totalCount + "...");
                
                var (city, temp) = ParseLine(line);
                
                if (cities.TryGetValue(city, out WeatherData? value))
                {
                    value.DataCount++;
                    value.Min = Math.Min(value.Min, temp);
                    value.Max = Math.Max(value.Max, temp);
                    value.Total += temp;
                    
                    cities[city] = value;
                }
                else
                {
                    cities.Add(city, new WeatherData(temp));
                }

                line = sr.ReadLine();
            }
            
            foreach (var (city, data) in cities)
            {
                Console.WriteLine($"{city}: Min: {data.Min}, Average: {data.Average} , Max: {data.Max}");
            }
            
            sw.Stop();
            sr.Close();
    
            Console.WriteLine("Done reading file." +
                              "\nTime taken: " + sw.ElapsedMilliseconds + "ms");
    
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static (string, float) ParseLine(string line)
    {
        var data = line.Split(';');
        return (data[0], float.Parse(data[1]));
    }
}
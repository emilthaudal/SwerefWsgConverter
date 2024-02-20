// See https://aka.ms/new-console-template for more information
using MightyLittleGeodesy.Positions;
using System.Data.Common;
using System.Diagnostics;

string Path = "input.csv";
string output = "output.csv";

Console.WriteLine($"Parsing {Path} to {output}");

var exists = File.Exists( Path );
if (!exists)
{
    Console.WriteLine($"File {Path} wasn't found. Add it to directory and try again");
    return;
}

Stopwatch stopwatch = Stopwatch.StartNew();

var lines = File.ReadLinesAsync(Path);

List<string> outputLines = [];

bool header = true;
await foreach (var item in lines)
{
    if (header)
    {
        header = false;
        continue;
    }

    var fields = item.Split(',');
    if (fields.Length != 3)
    {
        Console.WriteLine("Unexpected number of fields in line");
        return;
    }
    var identifier = fields[0];
    var longitude = fields[1];
    var latitude = fields[2];

    bool result = Double.TryParse(longitude, out double longi);
    if (!result)
    {
        Console.WriteLine($"Failed to parse longitude to double {longitude}");
    }

    result = Double.TryParse(latitude, out double lati);
    if (!result)
    {
        Console.WriteLine($"Failed to parse latitude to double {latitude}");
    }


    SWEREF99Position pos = new(longi, lati);
    var wgsPos = pos.ToWGS84();

    var outputLine = $"{identifier},{wgsPos.Latitude},{wgsPos.Latitude}";
    outputLines.Add( outputLine );
}

stopwatch.Stop();

File.Delete(output);

File.WriteAllLines(output, outputLines.ToArray());

Console.WriteLine($"Parsed {outputLines.Count} lines from {Path} to {output} in {stopwatch.Elapsed}");

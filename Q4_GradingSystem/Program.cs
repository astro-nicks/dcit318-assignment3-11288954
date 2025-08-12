using System;
using System.Collections.Generic;
using System.IO;

// Student class
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id; FullName = fullName; Score = score;
    }

    public string GetGrade()
    {
        return Score switch
        {
            >= 80 and <= 100 => "A",
            >= 70 and <= 79 => "B",
            >= 60 and <= 69 => "C",
            >= 50 and <= 59 => "D",
            _ => "F"
        };
    }

    public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
}

// Custom exceptions
public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string msg):base(msg){} }
public class MissingFieldException : Exception { public MissingFieldException(string msg):base(msg){} }

// Processor
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var list = new List<Student>();
        using var sr = new StreamReader(inputFilePath);
        string? line; int lineNo = 0;
        while ((line = sr.ReadLine()) != null)
        {
            lineNo++;
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 3) throw new MissingFieldException($"Line {lineNo}: missing fields.");
            if (!int.TryParse(parts[0], out var id)) throw new InvalidScoreFormatException($"Line {lineNo}: ID is invalid.");
            var name = parts[1];
            if (!int.TryParse(parts[2], out var score)) throw new InvalidScoreFormatException($"Line {lineNo}: score is invalid.");
            list.Add(new Student(id, name, score));
        }
        return list;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var sw = new StreamWriter(outputFilePath);
        foreach (var s in students) sw.WriteLine(s.ToString());
    }
}

class Program
{
    static void Main()
    {
        var input = "students_input.txt";   // create in project folder
        var output = "students_report.txt";

        // Sample input (create file if missing)
        if (!File.Exists(input))
        {
            File.WriteAllLines(input, new[]
            {
                "101, Kojo Essandoh, 84",
                "102, John Asante, 75",
                "103, Rexford Nkrumah, 62",
                // Add an intentional malformed example line to test exceptions if you want
                // "104, Dave, eighty"
            });
            Console.WriteLine($"Created sample {input}. Edit to test other scenarios.");
        }

        var processor = new StudentResultProcessor();
        try
        {
            var students = processor.ReadStudentsFromFile(input);
            processor.WriteReportToFile(students, output);
            Console.WriteLine($"Report written to {output}");
            Console.WriteLine("Report contents:");
            Console.WriteLine(File.ReadAllText(output));
        }
        catch (FileNotFoundException ex) { Console.WriteLine($"File not found: {ex.Message}"); }
        catch (InvalidScoreFormatException ex) { Console.WriteLine($"Invalid score: {ex.Message}"); }
        catch (MissingFieldException ex) { Console.WriteLine($"Missing field: {ex.Message}"); }
        catch (Exception ex) { Console.WriteLine($"Unexpected: {ex.Message}"); }
    }
}


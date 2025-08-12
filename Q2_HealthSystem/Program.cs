//using System;
//using System.Collections.Generic;
//using System.Linq;

// Generic Repository
public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => items;
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// Models
public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }

    public override string ToString() => $"{Id}: {Name}, {Age} years, {Gender}";
}

public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string medName, DateTime date)
    {
        Id = id; PatientId = patientId; MedicationName = medName; DateIssued = date;
    }

    public override string ToString() => $"{MedicationName} (Issued: {DateIssued:d})";
}

// App
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Bright Asante", 23, "Male"));
        _patientRepo.Add(new Patient(2, "Julia Forson", 21, "Female"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Paracetamol", DateTime.Now.AddDays(-15)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Vitamin C", DateTime.Now.AddDays(-7)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(4, 2, "Amoxicillin", DateTime.Now.AddDays(-2)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        foreach (var p in _patientRepo.GetAll())
            Console.WriteLine(p);
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        if (_prescriptionMap.ContainsKey(id))
            foreach (var pres in _prescriptionMap[id])
                Console.WriteLine(pres);
        else
            Console.WriteLine("No prescriptions found.");
    }
}

class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        Console.WriteLine("\nPrescriptions for Patient 1:");
        app.PrintPrescriptionsForPatient(1);
    }
}

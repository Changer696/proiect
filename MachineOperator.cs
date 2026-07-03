using System;

public class MachineOperator : Employee
{
    public MachineOperator(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.MachineOperator;
    }

    public void Opereaza(Machine masina)
    {

        if (masina.Status == MachineStatus.Running)
        {
            masina.Produce();
        }
        else if (masina.Status == MachineStatus.Maintenance)
        {
            Console.WriteLine("Masina este in mentenanta,asteptati sa fie reparata");
            return;
        }
        else
        {
            Console.WriteLine("Masina este oprita,nu puteti executa comanda");
            Console.WriteLine("Doriti sa porniti masina? DA/NU ");
            string continuare = Console.ReadLine();
            if (continuare == "DA")
            { 
                masina.Status = MachineStatus.Running;
                masina.Produce();
            }
            else if(continuare == "NU")
                {
                 return;
                }

        }
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Machine Operator) opereaza masinile.");
    }
}

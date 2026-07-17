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
            Console.WriteLine(Messages.MachineOperatorMaintenance);
            return;
        }
        else
        {
            Console.WriteLine(Messages.MachineOperatorStopped);
            Console.WriteLine(Messages.MachineStartConfirmation);
            string continuare = Console.ReadLine();
            if (continuare == Messages.Yes)
            { 
                masina.Status = MachineStatus.Running;
                masina.Produce();
            }
            else if(continuare == Messages.No)
                {
                 return;
                }

        }
    }
    // Operates the machine: starts it if needed and triggers production when running.
    public override void PerformDuty()
    {
        Console.WriteLine(Messages.MachineOperatorDuty(Nume));
    }
}
